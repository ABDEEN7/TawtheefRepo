using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval;

public record ProfileApprovalListItemDto
{
    public Guid UserProfileId { get; init; }
    public Guid UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? CandidateType { get; init; }
    public string? TargetEntity { get; init; }
    public string? Specialization { get; init; }
    public DateTimeOffset SubmittedAtUtc { get; init; }
    public UserProfileStatus ProfileStatus { get; init; }
    public int PendingCount { get; init; }
    public ReviewStatus OverallStatus { get; init; }
    public DateTimeOffset? LastUpdatedAtUtc { get; init; }
    public IReadOnlyList<string> AllowedOperations { get; init; } = [];
}
