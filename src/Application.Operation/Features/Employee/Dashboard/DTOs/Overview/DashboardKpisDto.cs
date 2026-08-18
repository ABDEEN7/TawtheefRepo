namespace Application.Operation.Features.Employee.Dashboard.DTOs.Overview;

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
