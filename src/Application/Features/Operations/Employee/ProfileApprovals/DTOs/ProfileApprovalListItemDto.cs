using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;

public record ProfileApprovalListItemDto
{
    public Guid UserProfileId { get; init; }
    public Guid UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? CandidateType { get; init; }
    public string? TargetEntity { get; init; }
    public DateTime SubmittedAtUtc { get; init; }
    public int PendingCount { get; init; }
    public ReviewStatus OverallStatus { get; init; }
    public DateTime? LastUpdatedAtUtc { get; init; }
}
