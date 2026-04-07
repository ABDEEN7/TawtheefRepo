using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.Dashboard.DTOs;

public sealed class OperationsDashboardDto
{
    public required string Role { get; init; }
    public required DashboardFiltersSnapshotDto Filters { get; init; }
    public required DashboardKpisDto Kpis { get; init; }
    public required ProfileBreakdownDto ProfileBreakdown { get; init; }
    public required JobKpisDto JobKpis { get; init; }
    public required JobBreakdownDto JobBreakdown { get; init; }
    public required TaskMonitoringDto TaskMonitoring { get; init; }
    public required TrendSeriesDto ProfileTrend { get; init; }
    public required TrendSeriesDto TaskCompletionTrend { get; init; }

    public required IReadOnlyList<PerformanceRankDto> TopPerformers { get; init; }
    public required IReadOnlyList<PerformanceRankDto> UnderPerformers { get; init; }
}

public sealed class DashboardFiltersSnapshotDto
{
    public DateTime? FromDateUtc { get; init; }
    public DateTime? ToDateUtc { get; init; }
    public Guid? DepartmentId { get; init; }
    public Guid? EmployeeId { get; init; }
    public string? Status { get; init; }
}

public sealed class DashboardKpisDto
{
    public int TotalEmployees { get; init; }
    public int ActiveEmployees { get; init; }
    public int TotalProfiles { get; init; }
    public int NewProfilesToday { get; init; }
    public int NewProfilesThisWeek { get; init; }
    public int NewProfilesThisMonth { get; init; }
    public int ApprovedProfiles { get; init; }
    public int RejectedProfiles { get; init; }
    public int PendingProfiles { get; init; }
    public int ReturnedProfiles { get; init; }
    public decimal ApprovalRate { get; init; }
    public decimal RejectionRate { get; init; }
    public decimal AverageApprovalHours { get; init; }
    public int TotalAssignedTasks { get; init; }
    public int RemainingTasks { get; init; }
    public int OverdueTasks { get; init; }
    public int FollowedMinisterOfficeCandidates { get; init; }
}

public sealed class ProfileBreakdownDto
{
    public IReadOnlyList<StatusCountDto> ByStatus { get; init; } = [];
    public IReadOnlyList<GroupCountDto> ByDepartment { get; init; } = [];
    public IReadOnlyList<GroupCountDto> ByPriority { get; init; } = [];
    public IReadOnlyList<AgingBucketDto> Aging { get; init; } = [];
}

public sealed class JobKpisDto
{
    public int TotalJobs { get; init; }
    public int ActiveJobs { get; init; }
    public int PendingReviewJobs { get; init; }
    public int ApprovedJobs { get; init; }
    public int RejectedJobs { get; init; }
    public int NewJobsToday { get; init; }
}

public sealed class JobBreakdownDto
{
    public IReadOnlyList<StatusCountDto> ByStatus { get; init; } = [];
    public IReadOnlyList<GroupCountDto> ByDepartment { get; init; } = [];
}

public sealed class TaskMonitoringDto
{
    public IReadOnlyList<GroupCountDto> TasksByDepartment { get; init; } = [];
    public IReadOnlyList<GroupCountDto> TasksByUrgency { get; init; } = [];
    public IReadOnlyList<GroupCountDto> TaskStatusStacked { get; init; } = [];
}

public sealed class TrendSeriesDto
{
    public IReadOnlyList<TrendPointDto> Points { get; init; } = [];
}

public sealed class TrendPointDto
{
    public required string Label { get; init; }
    public int Value { get; init; }
}

public sealed class TeamPerformanceRowDto
{
    public Guid EmployeeId { get; init; }
    public required string Name { get; init; }
    public string? EmployeeNumber { get; init; }
    public string? DepartmentName { get; init; }
    public string? JobDescription { get; init; }
    public int AssignedTasks { get; init; }
    public int ActiveTasks { get; init; }
    public int CompletedTasks { get; init; }
    public int RemainingTasks { get; init; }
    public int OverdueTasks { get; init; }
    public int ProfilesReviewed { get; init; }
    public decimal ApprovalRate { get; init; }
    public decimal RejectionRate { get; init; }
    public decimal AverageHandlingHours { get; init; }
    public decimal AverageResponseHours { get; init; }
    public decimal WorkloadRatio { get; init; }
    public string WorkloadBalanceIndicator { get; init; } = "Balanced";
    public decimal ProductivityScore { get; init; }
}

public sealed class PerformanceRankDto
{
    public Guid EmployeeId { get; init; }
    public required string Name { get; init; }
    public decimal Score { get; init; }
}

public sealed class StatusCountDto
{
    public required string Status { get; init; }
    public int Count { get; init; }
}

public sealed class GroupCountDto
{
    public required string Label { get; init; }
    public int Count { get; init; }
}

public sealed class AgingBucketDto
{
    public required string Bucket { get; init; }
    public int Count { get; init; }
}
