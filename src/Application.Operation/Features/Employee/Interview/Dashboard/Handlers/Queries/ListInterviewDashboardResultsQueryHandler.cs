using Application.Operation.Features.Employee.Interview.Dashboard.DTOs;
using Application.Operation.Features.Employee.Interview.Dashboard.Queries;
using Application.Operation.Features.Employee.Interview.Dashboard.Services;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Dashboard.Handlers.Queries;

// Interview results / hiring / waiting list / rejected report. Only the aggregate FinalScore the result
// report itself shows is exposed - the same data the Approve Interview page gives this permission.
public sealed class ListInterviewDashboardResultsQueryHandler(
    InterviewDashboardAccessResolver accessResolver,
    InterviewDashboardQueryScope scope)
    : IRequestHandler<ListInterviewDashboardResultsQuery, IResult<PaginatedResult<InterviewDashboardResultRowDto>>>
{
    public async Task<IResult<PaginatedResult<InterviewDashboardResultRowDto>>> Handle(
        ListInterviewDashboardResultsQuery request, CancellationToken cancellationToken)
    {
        var access = await accessResolver.ResolveAsync(cancellationToken);
        if (!access.Results)
            return InterviewDashboardPaging.Forbidden<InterviewDashboardResultRowDto>();

        var query = scope.ResultCandidates(request);
        query = request.Decision switch
        {
            null => query,
            InterviewDashboardDecisionView.Pending => query.Where(c => c.FinalDecision == null),
            var view => query.Where(c => c.FinalDecision == (FinalDecision)(int)view)
        };

        var rows = query
            .Select(c => new
            {
                c.Id,
                ScheduleId = c.InterviewResultReport!.InterviewScheduleId,
                ReportCode = c.InterviewResultReport.Code,
                ReportStatus = c.InterviewResultReport.Status,
                CandidateNameAr = c.InterviewAppointment!.Invitation!.Applicant!.FullNameAr,
                CandidateNameEn = c.InterviewAppointment.Invitation.Applicant.FullNameEn,
                JobNameAr = c.InterviewResultReport.InterviewSchedule!.Job!.JobTitle!.JobNameAr,
                JobNameEn = c.InterviewResultReport.InterviewSchedule.Job.JobTitle.JobNameEn,
                ScheduleTitleAr = c.InterviewResultReport.InterviewSchedule.TitleAr,
                ScheduleTitleEn = c.InterviewResultReport.InterviewSchedule.TitleEn,
                c.FinalScore,
                c.QualificationScore,
                c.IsQualified,
                c.FinalDecision,
                c.DecidedAt,
                ReportCreated = c.InterviewResultReport.CreatedDate
            })
            .OrderByDescending(r => r.ReportCreated)
            .ThenByDescending(r => r.FinalScore)
            .ThenBy(r => r.Id);

        return await InterviewDashboardPaging.ToPageAsync(rows, r => new InterviewDashboardResultRowDto(
            r.Id, r.ScheduleId, r.ReportCode, r.ReportStatus, r.CandidateNameAr, r.CandidateNameEn,
            r.JobNameAr, r.JobNameEn, r.ScheduleTitleAr, r.ScheduleTitleEn, r.FinalScore, r.QualificationScore,
            r.IsQualified, r.FinalDecision, r.DecidedAt.AsUtcOffset()), request, cancellationToken);
    }
}
