using Application.Operation.Features.Employee.Dashboard.DTOs.Candidates;
using Application.Operation.Features.Employee.Dashboard.DTOs.Employees;
using Application.Operation.Features.Employee.Dashboard.DTOs.Invitations;
using Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;

namespace Application.Operation.Features.Employee.Dashboard.DTOs.Overview;

public sealed class DashboardOverviewDto
{
    public required string Role { get; init; }
    public required DashboardFiltersSnapshotDto Filters { get; init; }
    public required DashboardKpisDto Kpis { get; init; }
    public required ProfileBreakdownDto ProfileBreakdown { get; init; }
    public required CandidateTypeKpisDto CandidateTypeKpis { get; init; }
    public required JobKpisDto JobKpis { get; init; }
    public required InvitationKpisDto InvitationKpis { get; init; }
    public required JobBreakdownDto JobBreakdown { get; init; }
    public required TaskMonitoringDto TaskMonitoring { get; init; }
}
