namespace Application.Operation.Features.Employee.Dashboard.DTOs;

public sealed class OperationsDashboardDto
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
    public required TrendSeriesDto ProfileTrend { get; init; }
    public required TrendSeriesDto TaskCompletionTrend { get; init; }
    public required IReadOnlyList<LatestJobDto> LatestJobs { get; init; }

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
    public int InCreationProfiles { get; init; }
    public int SubmittedProfiles { get; init; }
    public int UnderReviewProfiles { get; init; }
    public int PendingProfiles { get; init; }
    public int ReturnedProfiles { get; init; }
    public decimal ApprovalRate { get; init; }
    public decimal RejectionRate { get; init; }
    public decimal AverageApprovalHours { get; init; }
    public int TotalAssignedTasks { get; init; }
    public int CompletedTasks { get; init; }
    public int RemainingTasks { get; init; }
    public int OverdueTasks { get; init; }
    public int UnassignedProfiles { get; init; }
    public int FollowedMinisterOfficeCandidates { get; init; }
}

public sealed class ProfileBreakdownDto
{
    public IReadOnlyList<StatusCountDto> ByStatus { get; init; } = [];
    public IReadOnlyList<GroupCountDto> ByDepartment { get; init; } = [];
    public IReadOnlyList<GroupCountDto> ByPriority { get; init; } = [];
    public IReadOnlyList<CandidateTypeCountDto> ByCandidateType { get; init; } = [];
    public IReadOnlyList<AgingBucketDto> Aging { get; init; } = [];
}

public sealed class JobKpisDto
{
    public int TotalJobs { get; init; }
    public int DraftJobs { get; init; }
    public int ActiveJobs { get; init; }
    public int PendingReviewJobs { get; init; }
    public int ApprovedJobs { get; init; }
    public int RejectedJobs { get; init; }
    public int NewJobsToday { get; init; }
    public int PendingPointConfigurationJobs { get; init; }
    public int NeedPointUpdateJobs { get; init; }
    public int PendingPointApprovalJobs { get; init; }
    public int NeedUpdateJobs { get; init; }
    public int ReadyForAnnouncementJobs { get; init; }
    public int PublishedJobs { get; init; }
    public int ClosedJobs { get; init; }
    public int CancelledJobs { get; init; }
}

public sealed class InvitationKpisDto
{
    public int TotalInvitations { get; init; }
    public int AcceptedInvitations { get; init; }
    public int PendingInvitations { get; init; }
    public int PendingAttachmentApproval { get; init; }
}

public sealed class JobBreakdownDto
{
    public IReadOnlyList<StatusCountDto> ByStatus { get; init; } = [];
    public IReadOnlyList<GroupCountDto> ByDepartment { get; init; } = [];
}

public sealed class LatestJobDto
{
    public Guid JobId { get; init; }
    public required string JobTitle { get; init; }
    public required string ManagementName { get; init; }
    public required string Status { get; init; }
    public int CandidatesCount { get; init; }
    public int InvitationsSent { get; init; }
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

public sealed class CandidateTypeCountDto
{
    public Guid? CandidateTypeId { get; init; }
    public required string Key { get; init; }
    public required string Label { get; init; }
    public int Count { get; init; }
}

public sealed class CandidateTypeKpisDto
{
    public int Total { get; init; }
    public int Qatari { get; init; }
    public int NonQatari { get; init; }
    public int SonOfQatariMother { get; init; }
    public int WifeOfQatari { get; init; }
    public int Gcc { get; init; }
    public int ResidentQatar { get; init; }
    public int Unknown { get; init; }
}

public sealed class AgingBucketDto
{
    public required string Bucket { get; init; }
    public int Count { get; init; }
}
