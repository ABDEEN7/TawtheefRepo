using Application.Operation.Features.Employee.Interview.ResultReport.DTOs;
using Application.Operation.Features.Employee.Interview.ResultReport.Queries;
using Application.Operation.Features.Employee.Interview.ResultReport.Services;
using Application.Operation.Features.Employee.Interview.OperationalIssue.DTOs;
using FluentResults;
using Tawtheef.Application.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.Interview.ResultReport.Handlers.Queries;

public sealed class GetResultReportByScheduleQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetResultReportByScheduleQuery, IResult<ResultReportDto>>
{
    public async Task<IResult<ResultReportDto>> Handle(GetResultReportByScheduleQuery request, CancellationToken cancellationToken)
    {
        var report = await unitOfWork.GetEntityRepository<InterviewResultReport>().DbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(r => r.InterviewSchedule).ThenInclude(s => s!.Job).ThenInclude(j => j!.JobTitle)
            .Include(r => r.Candidates).ThenInclude(c => c.Axes).ThenInclude(a => a.InterviewTemplateEvaluationAxis).ThenInclude(ax => ax!.InterviewEvaluationAxis)
            .Include(r => r.Candidates).ThenInclude(c => c.InterviewAppointment).ThenInclude(a => a!.Invitation).ThenInclude(i => i!.Applicant).ThenInclude(a => a!.Profile)
            .Include(r => r.Candidates).ThenInclude(c => c.RecommendedSchoolStage)
            .FirstOrDefaultAsync(r => r.InterviewScheduleId == request.ScheduleId, cancellationToken);

        if (report is null)
            return Result.Fail<ResultReportDto>(new Error(ErrorsCodes.InterviewResultReportNotFound));

        var numberOfVacancies = report.InterviewSchedule!.Job!.NumberOfVacancies;
        var alreadyHiringCountForJob = await ResultReportReadService.CountAlreadyHiringForJobAsync(
            unitOfWork, report.InterviewSchedule.JobId, report.Id, cancellationToken);
        var suggestions = ResultCandidateSuggestionService.Suggest(
            report.Candidates.Select(c => new SuggestionCandidate(
                c.Id,
                c.IsQualified,
                c.InterviewAppointment?.Invitation?.Applicant?.Profile?.NationalityId == CountryIds.Qatar,
                c.FinalScore)),
            numberOfVacancies,
            alreadyHiringCountForJob);

        // So the Final Reviewer sees which candidates have operational issues without a second call
        // (scheduleResult.md: "must see which candidates have operational issues and review them individually").
        var issuesByAppointment = await ResultReportReadService.LoadIssuesByAppointmentAsync(
            unitOfWork, report.Candidates.Select(c => c.InterviewAppointmentId).ToList(), cancellationToken);

        var candidates = report.Candidates
            .Select(c =>
            {
                var suggestedDecision = suggestions[c.Id];
                var issues = issuesByAppointment.GetValueOrDefault(c.InterviewAppointmentId, []);
                var attendanceStatus = c.InterviewAppointment?.AttendanceStatus;

                return new ResultCandidateDto(
                    c.Id,
                    c.InterviewAppointmentId,
                    c.InterviewAppointment?.Invitation?.Applicant?.FullNameAr ?? string.Empty,
                    c.InterviewAppointment?.Invitation?.Applicant?.FullNameEn,
                    c.InterviewAppointment?.Invitation?.Applicant?.Profile?.NationalNumber,
                    c.FinalScore,
                    c.QualificationScore,
                    c.IsQualified,
                    attendanceStatus,
                    InterviewAppointment.IsLateCandidate(attendanceStatus, issues.Select(i => i.IssueType)),
                    c.FinalDecision,
                    c.DecisionReason,
                    suggestedDecision,
                    c.ChairRecommendedDecision,
                    c.ChairRecommendationReason,
                    c.RecommendedSchoolStageId,
                    c.RecommendedSchoolStage?.NameAr,
                    c.RecommendedSchoolStage?.NameEn,
                    c.SnapshotAt.AsUtcOffset(),
                    issues,
                    c.Axes
                        .Select(a => new ResultCandidateAxisDto(
                            a.InterviewTemplateEvaluationAxisId,
                            a.InterviewTemplateEvaluationAxis?.InterviewEvaluationAxis?.NameAr,
                            a.InterviewTemplateEvaluationAxis?.InterviewEvaluationAxis?.NameEn,
                            a.Score,
                            a.QualificationScore,
                            a.QualificationMet))
                        .ToList());
            })
            .ToList();

        var dto = new ResultReportDto(
            report.Id,
            report.Code,
            report.InterviewScheduleId,
            report.InterviewSchedule.TitleAr,
            report.InterviewSchedule.TitleEn,
            report.InterviewSchedule.Job.JobTitle!.JobNameAr,
            report.InterviewSchedule.Job.JobTitle.JobNameEn,
            report.AppliedQualificationScore,
            report.Status,
            report.ApprovedById,
            report.ApprovedAt.AsUtcOffset(),
            report.DecisionNotes,
            candidates);

        return Result.Ok(dto);
    }
}
