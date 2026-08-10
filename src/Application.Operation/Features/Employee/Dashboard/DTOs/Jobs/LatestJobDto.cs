namespace Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;

public sealed class LatestJobDto
{
    public Guid JobId { get; init; }
    public required string JobTitle { get; init; }
    public required string ManagementName { get; init; }
    public required string Status { get; init; }
    public int CandidatesCount { get; init; }
    public int InvitationsSent { get; init; }
}
