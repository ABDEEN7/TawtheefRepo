using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;

public record ProfileApprovalSectionDto
{
    public ProfileSection Section { get; init; }
    public ReviewStatus Status { get; init; } = ReviewStatus.Pending;
    public string? Note { get; init; }
    public DateTimeOffset? ReviewedAtUtc { get; init; }
    public ProfileApprovalItemDto? SectionReview { get; init; }
    public IReadOnlyList<ProfileApprovalItemDto> Items { get; init; } = [];
    public bool HasAttachments { get; init; }
}
