namespace Application.Operation.Features.Employee.Dashboard.DTOs.Overview;

public sealed class DashboardMetricTrendDto
{
    public int PreviousValue { get; init; }
    public decimal? ChangePercentage { get; init; }
}

public sealed class DashboardKpiTrendsDto
{
    public required DashboardMetricTrendDto TotalProfiles { get; init; }
    public required DashboardMetricTrendDto ApprovedProfiles { get; init; }
    public required DashboardMetricTrendDto UnderReviewProfiles { get; init; }
    public required DashboardMetricTrendDto UnassignedProfiles { get; init; }
    public required DashboardMetricTrendDto PublishedJobs { get; init; }
    public required DashboardMetricTrendDto TotalInvitations { get; init; }
    public required DashboardMetricTrendDto AcceptedInvitations { get; init; }
}
