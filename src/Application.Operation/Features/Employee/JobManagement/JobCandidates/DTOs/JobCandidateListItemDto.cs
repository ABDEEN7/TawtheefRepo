namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;

public sealed class JobCandidateListItemDto
{
    public Guid? InvitationId { get; init; }
    public Guid CandidateId { get; init; }
    public string CandidateName { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public string JobCategory { get; init; } = string.Empty;
    public string CandidateCategory { get; init; } = string.Empty;
    public string CandidateMajor { get; init; } = string.Empty;
    public string CandidateGender { get; init; } = string.Empty;
    public int Points { get; init; }
}
