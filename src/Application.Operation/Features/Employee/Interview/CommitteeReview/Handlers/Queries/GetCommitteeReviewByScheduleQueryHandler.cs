using Application.Operation.Features.Employee.Interview.CommitteeReview.DTOs;
using Application.Operation.Features.Employee.Interview.CommitteeReview.Queries;
using Application.Operation.Features.Employee.Interview.Evaluation.Services;
using Application.Operation.Features.Employee.Interview.ResultReport.DTOs;
using Application.Operation.Features.Employee.Interview.ResultReport.Services;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Interview.CommitteeReview.Handlers.Queries;

// Everything the approval screen shows for the schedule's result report, plus each committee member's
// evaluation and the per-criterion scores behind the stored axis/final scores. Nothing here is
// recalculated into the report - the stored scores stay the ones generated with the report.
public sealed class GetCommitteeReviewByScheduleQueryHandler(
    IUnitOfWork unitOfWork, EvaluationAccessResolver accessResolver, UserManager<User> userManager)
    : IRequestHandler<GetCommitteeReviewByScheduleQuery, IResult<CommitteeReviewDto>>
{
    public async Task<IResult<CommitteeReviewDto>> Handle(GetCommitteeReviewByScheduleQuery request, CancellationToken cancellationToken)
    {
        var report = await unitOfWork.GetEntityRepository<InterviewResultReport>().DbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(r => r.InterviewSchedule).ThenInclude(s => s!.Job).ThenInclude(j => j!.JobTitle)
            .Include(r => r.Candidates).ThenInclude(c => c.Axes).ThenInclude(a => a.InterviewTemplateEvaluationAxis).ThenInclude(ax => ax!.InterviewEvaluationAxis)
            .Include(r => r.Candidates).ThenInclude(c => c.InterviewAppointment).ThenInclude(a => a!.Invitation).ThenInclude(i => i!.Applicant).ThenInclude(a => a!.Profile)
            .FirstOrDefaultAsync(r => r.InterviewScheduleId == request.ScheduleId, cancellationToken);

        if (report is null || report.Candidates.Count == 0)
            return Result.Fail<CommitteeReviewDto>(new Error(ErrorsCodes.InterviewResultReportNotFound));

        var committeeId = report.Candidates.First().InterviewAppointment!.InterviewCommitteeId;
        if (!await accessResolver.IsChairOrBypassAsync(committeeId, cancellationToken))
            return Result.Fail<CommitteeReviewDto>(new Error(ErrorsCodes.InterviewCommitteeReviewForbidden)
                .WithMetadata("StatusCode", StatusCodes.Status403Forbidden));

        var canEdit = report.IsCommitteeReviewEditable;
        var canOverride = canEdit && accessResolver.HasPermission(PermissionKeys.InterviewCommitteeReview.OverrideSuggestion);

        // The evaluation-form structure - the same approved template version the report was calculated from.
        var committee = await unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet
            .AsNoTracking()
            .FirstAsync(c => c.Id == committeeId, cancellationToken);
        var versionResult = await EvaluationTemplateResolver.GetApprovedVersionAsync(unitOfWork, committee.InterviewTemplateId, cancellationToken);
        var axes = versionResult.IsFailed
            ? []
            : versionResult.Value.Axes
                .OrderBy(a => a.OrderNo)
                .Select(a => new CommitteeReviewAxisDto(
                    a.Id,
                    a.InterviewEvaluationAxis?.NameAr,
                    a.InterviewEvaluationAxis?.NameEn,
                    a.MaxScore,
                    a.QualificationScore,
                    a.Criteria
                        .OrderBy(c => c.OrderNo)
                        .Select(c => new CommitteeReviewCriterionDto(
                            c.Id,
                            c.NameAr ?? c.InterviewEvaluationCriterion?.NameAr,
                            c.NameEn ?? c.InterviewEvaluationCriterion?.NameEn,
                            c.MaxScore))
                        .ToList()))
                .ToList();

        // Chair first, then the other members. User is IdentityUser-based, so names are matched in
        // memory (same as ListAppointmentEvaluationsQueryHandler).
        var members = await unitOfWork.GetEntityRepository<InterviewCommitteeMember>().DbSet
            .AsNoTracking()
            .Where(m => m.InterviewCommitteeId == committeeId && m.IsActive)
            .OrderBy(m => m.Role)
            .Select(m => new { m.Id, m.MemberUserId, m.Role })
            .ToListAsync(cancellationToken);
        var memberUserIds = members.Select(m => m.MemberUserId).ToList();
        var users = await userManager.Users.Where(u => memberUserIds.Contains(u.Id)).ToListAsync(cancellationToken);

        var appointmentIds = report.Candidates.Select(c => c.InterviewAppointmentId).ToList();
        var evaluations = await unitOfWork.GetEntityRepository<InterviewMemberEvaluation>().DbSet
            .AsNoTracking()
            .Include(e => e.CriterionScores)
            .Where(e => appointmentIds.Contains(e.InterviewAppointmentId))
            .ToListAsync(cancellationToken);

        var numberOfVacancies = report.InterviewSchedule!.Job!.NumberOfVacancies;
        var alreadyHiringCountForJob = await ResultReportReadService.CountAlreadyHiringForJobAsync(
            unitOfWork, report.InterviewSchedule.JobId, cancellationToken);
        var issuesByAppointment = await ResultReportReadService.LoadIssuesByAppointmentAsync(unitOfWork, appointmentIds, cancellationToken);

        var candidates = report.Candidates
            .OrderBy(c => c.InterviewAppointment?.StartAt)
            .Select(c =>
            {
                var applicant = c.InterviewAppointment?.Invitation?.Applicant;
                var isQatari = applicant?.Profile?.NationalityId == CountryIds.Qatar;
                var issues = issuesByAppointment.GetValueOrDefault(c.InterviewAppointmentId, []);
                var attendanceStatus = c.InterviewAppointment?.AttendanceStatus;
                var candidateEvaluations = evaluations.Where(e => e.InterviewAppointmentId == c.InterviewAppointmentId).ToList();

                // Only submitted evaluations count - the same set the result calculation used.
                var criterionAverages = candidateEvaluations
                    .Where(e => e.Status == MemberEvaluationStatus.Submitted)
                    .SelectMany(e => e.CriterionScores)
                    .GroupBy(cs => cs.InterviewTemplateEvaluationCriterionId)
                    .Select(g => new CommitteeReviewCriterionScoreDto(g.Key, g.Average(cs => cs.Score)))
                    .ToList();

                var memberDtos = members
                    .Select(m =>
                    {
                        var user = users.FirstOrDefault(u => u.Id == m.MemberUserId);
                        var evaluation = candidateEvaluations.FirstOrDefault(e => e.InterviewCommitteeMemberId == m.Id);
                        var scores = evaluation?.Status == MemberEvaluationStatus.Submitted
                            ? evaluation.CriterionScores
                                .Select(cs => new CommitteeReviewCriterionScoreDto(cs.InterviewTemplateEvaluationCriterionId, cs.Score))
                                .ToList()
                            : [];
                        return new CommitteeReviewMemberDto(
                            m.Id,
                            user?.FullNameAr ?? string.Empty,
                            user?.FullNameEn,
                            m.Role,
                            evaluation?.Status,
                            evaluation?.TotalScore,
                            evaluation?.SubmittedAt.AsUtcOffset(),
                            scores);
                    })
                    .ToList();

                return new CommitteeReviewCandidateDto(
                    c.Id,
                    c.InterviewAppointmentId,
                    applicant?.FullNameAr ?? string.Empty,
                    applicant?.FullNameEn,
                    applicant?.Profile?.NationalNumber,
                    c.FinalScore,
                    c.QualificationScore,
                    c.IsQualified,
                    attendanceStatus,
                    InterviewAppointment.IsLateCandidate(attendanceStatus, issues.Select(i => i.IssueType)),
                    ResultCandidateSuggestionService.Suggest(c.IsQualified, isQatari, numberOfVacancies, alreadyHiringCountForJob),
                    c.ChairRecommendedDecision,
                    c.ChairRecommendationReason,
                    c.RecommendedSchoolStageId,
                    issues,
                    c.Axes
                        .Select(a => new ResultCandidateAxisDto(
                            a.InterviewTemplateEvaluationAxisId,
                            a.InterviewTemplateEvaluationAxis?.InterviewEvaluationAxis?.NameAr,
                            a.InterviewTemplateEvaluationAxis?.InterviewEvaluationAxis?.NameEn,
                            a.Score,
                            a.QualificationScore,
                            a.QualificationMet))
                        .ToList(),
                    criterionAverages,
                    memberDtos);
            })
            .ToList();

        return Result.Ok(new CommitteeReviewDto(
            report.Id,
            report.Code,
            report.InterviewScheduleId,
            report.InterviewSchedule.TitleAr,
            report.InterviewSchedule.TitleEn,
            report.InterviewSchedule.Job.JobTitle!.JobNameAr,
            report.InterviewSchedule.Job.JobTitle.JobNameEn,
            report.AppliedQualificationScore,
            report.Status,
            report.CommitteeReviewedAt.AsUtcOffset(),
            canEdit,
            canOverride,
            axes,
            candidates));
    }
}
