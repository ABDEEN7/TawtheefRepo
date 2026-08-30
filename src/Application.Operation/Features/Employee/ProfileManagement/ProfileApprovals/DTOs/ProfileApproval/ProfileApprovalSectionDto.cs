using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval;

public record ProfileApprovalSectionDto
{
    public ProfileSection Section { get; init; }
    public ReviewStatus Status { get; init; } = ReviewStatus.Pending;
    public string? Note { get; init; }
    public DateTimeOffset? ReviewedAtUtc { get; init; }
    public ProfileApprovalItemDto? SectionReview { get; init; }
    public IReadOnlyList<ProfileApprovalItemDto> Items { get; init; } = [];
    public bool HasAttachments { get; init; }
    public string? InternalReviewerNote { get; init; }
}
