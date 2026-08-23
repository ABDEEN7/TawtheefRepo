namespace Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;

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
