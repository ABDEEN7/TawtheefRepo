namespace Application.Operation.Features.Employee.Dashboard.DTOs;

public sealed class DashboardOverviewDto
{
    public required string Role { get; init; }
    public required DashboardFiltersSnapshotDto Filters { get; init; }
    public required DashboardKpisDto Kpis { get; init; }
    public required JobKpisDto JobKpis { get; init; }
    public required InvitationKpisDto InvitationKpis { get; init; }
}

public sealed class CandidateStatusSummaryDto
{
    public int TotalProfiles { get; init; }
    public int InCreationProfiles { get; init; }
    public int SubmittedProfiles { get; init; }
    public int UnderReviewProfiles { get; init; }
    public int ApprovedProfiles { get; init; }
    public int ReturnedProfiles { get; init; }
    public int RejectedProfiles { get; init; }
    public required ProfileBreakdownDto ProfileBreakdown { get; init; }
}

public sealed class CandidateTypeSummaryDto
{
    public required CandidateTypeKpisDto CandidateTypeKpis { get; init; }
    public required IReadOnlyList<CandidateTypeCountDto> ByCandidateType { get; init; }
}

public sealed class JobsSummaryDto
{
    public required JobKpisDto JobKpis { get; init; }
    public required InvitationKpisDto InvitationKpis { get; init; }
    public required JobBreakdownDto JobBreakdown { get; init; }
}

public sealed class EmployeeIndicatorsDto
{
    public int ActiveEmployees { get; init; }
    public int RemainingTasks { get; init; }
    public int UnassignedProfiles { get; init; }
    public int CompletedTasks { get; init; }
}

public sealed class EmployeeReviewOutcomesDto
{
    public int TotalProfiles { get; init; }
    public int ApprovedProfiles { get; init; }
    public int ReturnedProfiles { get; init; }
    public int UnassignedProfiles { get; init; }
    public int PendingProfiles { get; init; }
}
