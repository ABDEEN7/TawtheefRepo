namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;

public record ProfileApprovalDetailDto
{
    public Guid UserProfileId { get; init; }
    public Guid UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? CandidateType { get; init; }
    public string? TargetEntity { get; init; }
    public int? SubmissionVersion { get; init; }
    public DateTime? SubmittedAtUtc { get; init; }
    public ProfileApprovalDataDto? Profile { get; init; }
    public IReadOnlyList<ProfileApprovalSectionDto> Sections { get; init; } = Array.Empty<ProfileApprovalSectionDto>();
}
