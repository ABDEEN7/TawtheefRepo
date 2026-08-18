using Application.Operation.Features.Employee.Dashboard.DTOs.Candidates;
using Application.Operation.Features.Employee.Dashboard.DTOs.Invitations;
using Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;

namespace Application.Operation.Features.Employee.Dashboard.Services.Read;

internal sealed record PeriodValues<T>(T Current, T Previous);

internal sealed record DashboardProfileMetrics(
    IReadOnlyList<CandidateTypeCountDto> CandidateTypes,
    PeriodValues<Dictionary<string, int>> Statuses,
    PeriodValues<int> Unassigned,
    int Rejected,
    int NewToday,
    int NewThisWeek,
    int NewThisMonth,
    decimal AverageApprovalHours,
    int FollowedMinisterOfficeCandidates);

internal sealed record DashboardJobMetrics(
    PeriodValues<Dictionary<string, int>> Statuses,
    PeriodValues<JobKpisDto> Kpis,
    IReadOnlyList<JobStatusCountDto> CurrentStatusBreakdown);

internal sealed record DashboardInvitationMetrics(PeriodValues<InvitationKpisDto> Kpis);

internal sealed record DashboardTaskCounts(int Total, int Completed, int Remaining, int Overdue);

internal sealed record DashboardEmployeeWorkload(
    int AwaitingDistribution,
    int Returned,
    int Completed,
    int InProgress,
    int Submitted);

internal sealed record DashboardWorkloadMetrics(
    DashboardTaskCounts Tasks,
    DashboardEmployeeWorkload Workload,
    int Employees);
