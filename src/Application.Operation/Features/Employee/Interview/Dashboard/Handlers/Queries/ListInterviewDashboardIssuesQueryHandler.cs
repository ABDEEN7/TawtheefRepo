using Application.Operation.Features.Employee.Interview.Dashboard.DTOs;
using Application.Operation.Features.Employee.Interview.Dashboard.Queries;
using Application.Operation.Features.Employee.Interview.Dashboard.Services;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;

namespace Application.Operation.Features.Employee.Interview.Dashboard.Handlers.Queries;

// Operational issues report - same visibility as the operational-issues dialog (execution access).
public sealed class ListInterviewDashboardIssuesQueryHandler(
    InterviewDashboardAccessResolver accessResolver,
    InterviewDashboardQueryScope scope)
    : IRequestHandler<ListInterviewDashboardIssuesQuery, IResult<PaginatedResult<InterviewDashboardIssueRowDto>>>
{
    public async Task<IResult<PaginatedResult<InterviewDashboardIssueRowDto>>> Handle(
        ListInterviewDashboardIssuesQuery request, CancellationToken cancellationToken)
    {
        var access = await accessResolver.ResolveAsync(cancellationToken);
        if (!access.Execution)
            return InterviewDashboardPaging.Forbidden<InterviewDashboardIssueRowDto>();

        var query = scope.Issues(request, access);
        if (request.IssueStatus.HasValue)
            query = query.Where(i => i.Status == request.IssueStatus.Value);

        var rows = query
            .Select(i => new
            {
                i.Id,
                ScheduleId = i.InterviewAppointment!.InterviewScheduleId,
                i.InterviewAppointmentId,
                i.IssueType,
                i.Status,
                i.IsBlocking,
                i.Description,
                CandidateNameAr = i.InterviewAppointment.Invitation != null ? i.InterviewAppointment.Invitation.Applicant!.FullNameAr : null,
                CandidateNameEn = i.InterviewAppointment.Invitation != null ? i.InterviewAppointment.Invitation.Applicant!.FullNameEn : null,
                ScheduleTitleAr = i.InterviewAppointment.InterviewSchedule!.TitleAr,
                ScheduleTitleEn = i.InterviewAppointment.InterviewSchedule.TitleEn,
                i.CreatedDate,
                i.ResolvedAt
            })
            .OrderBy(r => r.Status)
            .ThenByDescending(r => r.IsBlocking)
            .ThenByDescending(r => r.CreatedDate);

        return await InterviewDashboardPaging.ToPageAsync(rows, r => new InterviewDashboardIssueRowDto(
            r.Id, r.ScheduleId, r.InterviewAppointmentId, r.IssueType, r.Status, r.IsBlocking, r.Description,
            r.CandidateNameAr, r.CandidateNameEn, r.ScheduleTitleAr, r.ScheduleTitleEn,
            r.CreatedDate.AsUtcOffset(), r.ResolvedAt.AsUtcOffset()), request, cancellationToken);
    }
}
