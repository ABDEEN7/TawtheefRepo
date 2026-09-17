using Application.Operation.Features.Employee.Interview.ResultReport.DTOs;
using Application.Operation.Features.Employee.Interview.ResultReport.Queries;
using Application.Operation.Features.Employee.Interview.ResultReport.Services;
using Application.Operation.Features.Employee.Interview.OperationalIssue.DTOs;
using FluentResults;
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
            .FirstOrDefaultAsync(r => r.InterviewScheduleId == request.ScheduleId, cancellationToken);

        if (report is null)
            return Result.Fail<ResultReportDto>(new Error(ErrorsCodes.InterviewResultReportNotFound));

        var jobId = report.InterviewSchedule!.JobId;
        var numberOfVacancies = report.InterviewSchedule.Job!.NumberOfVacancies;

        // Scoped to the whole job (across every schedule/report for it), not just this report - a
        // vacancy is filled once, regardless of which interview round the candidate came through.
        var alreadyHiringCountForJob = await unitOfWork.GetEntityRepository<InterviewResultCandidate>().DbSet
            .Where(c => c.FinalDecision == FinalDecision.CandidateForHiringProcess
                && c.InterviewResultReport!.InterviewSchedule!.JobId == jobId)
            .CountAsync(cancellationToken);

        var appointmentIds = report.Candidates.Select(c => c.InterviewAppointmentId).ToList();

        // So the Final Reviewer sees which candidates have operational issues without a second call
        // (scheduleResult.md: "must see which candidates have operational issues and review them individually").
        var issuesByAppointment = (await unitOfWork.GetEntityRepository<InterviewOperationalIssue>().DbSet
            .AsNoTracking()
            .Where(i => appointmentIds.Contains(i.InterviewAppointmentId))
            .OrderByDescending(i => i.CreatedDate)
            .Select(i => new OperationalIssueDto(
                i.Id, i.InterviewAppointmentId, i.IssueType, i.Description, i.IsBlocking, i.Status,
                i.ResolvedById, i.ResolvedAt, i.ResolutionNotes, i.CreatedDate))
            .ToListAsync(cancellationToken))
            .GroupBy(i => i.InterviewAppointmentId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var candidates = report.Candidates
            .Select(c =>
            {
                var isQatari = c.InterviewAppointment?.Invitation?.Applicant?.Profile?.NationalityId == CountryIds.Qatar;
                var suggestedDecision = ResultCandidateSuggestionService.Suggest(
                    c.IsQualified, isQatari, numberOfVacancies, alreadyHiringCountForJob);

                return new ResultCandidateDto(
                    c.Id,
                    c.InterviewAppointmentId,
                    c.InterviewAppointment?.Invitation?.Applicant?.FullNameAr ?? string.Empty,
                    c.InterviewAppointment?.Invitation?.Applicant?.FullNameEn,
                    c.FinalScore,
                    c.QualificationScore,
                    c.IsQualified,
                    c.FinalDecision,
                    suggestedDecision,
                    c.SnapshotAt,
                    issuesByAppointment.GetValueOrDefault(c.InterviewAppointmentId, []),
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
            report.InterviewScheduleId,
            report.InterviewSchedule.TitleAr,
            report.InterviewSchedule.TitleEn,
            report.InterviewSchedule.Job.JobTitle!.JobNameAr,
            report.InterviewSchedule.Job.JobTitle.JobNameEn,
            report.AppliedQualificationScore,
            report.Status,
            report.ApprovedById,
            report.ApprovedAt,
            report.DecisionNotes,
            candidates);

        return Result.Ok(dto);
    }
}
