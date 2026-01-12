namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;

public sealed class SendJobCandidateInvitationsResult
{
    public int TotalTargets { get; init; }
    public int SentEmailCount { get; init; }
    public int SentSmsCount { get; init; }
    public int UpdatedStatusCount { get; init; }
}
