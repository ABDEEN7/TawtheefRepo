using Application.Operation.Features.Employee.Interview.Dashboard.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Dashboard.Queries;

public sealed record GetInterviewDashboardOverviewQuery : InterviewDashboardFilter,
    IRequest<IResult<InterviewDashboardOverviewDto>>;

public sealed record ListInterviewDashboardSchedulesQuery : InterviewDashboardPagedFilter,
    IRequest<IResult<PaginatedResult<InterviewDashboardScheduleRowDto>>>;

public sealed record ListInterviewDashboardCandidatesQuery : InterviewDashboardPagedFilter,
    IRequest<IResult<PaginatedResult<InterviewDashboardCandidateRowDto>>>
{
    // Table-local quick filter on top of the dashboard filters. NotRecorded = attendance still empty.
    public InterviewDashboardAttendanceView? Attendance { get; init; }
}

public sealed record ListInterviewDashboardResultsQuery : InterviewDashboardPagedFilter,
    IRequest<IResult<PaginatedResult<InterviewDashboardResultRowDto>>>
{
    // Table-local quick filter; Pending = in a report but no final decision recorded yet.
    public InterviewDashboardDecisionView? Decision { get; init; }
}

public sealed record ListInterviewDashboardIssuesQuery : InterviewDashboardPagedFilter,
    IRequest<IResult<PaginatedResult<InterviewDashboardIssueRowDto>>>
{
    public OperationalIssueStatus? IssueStatus { get; init; }
}

public sealed record ListInterviewDashboardLookupsQuery(InterviewDashboardLookupKind Kind, string? Search, Guid? Id, Guid? JobId)
    : IRequest<IResult<List<InterviewDashboardLookupDto>>>;

public enum InterviewDashboardAttendanceView
{
    Present = 1,
    NoShow = 2,
    Withdrew = 3,
    Late = 4,
    NotRecorded = 5
}

public enum InterviewDashboardDecisionView
{
    CandidateForHiringProcess = 1,
    WaitingList = 2,
    Rejected = 3,
    NeedsAction = 4,
    NoShow = 5,
    Pending = 6
}

public enum InterviewDashboardLookupKind
{
    Job = 1,
    Committee = 2,
    Schedule = 3
}
