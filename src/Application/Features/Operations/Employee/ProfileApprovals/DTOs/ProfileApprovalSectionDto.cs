using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;

public record ProfileApprovalSectionDto
{
    public ProfileSection Section { get; init; }
    public ProfileApprovalItemDto? SectionReview { get; init; }
    public IReadOnlyList<ProfileApprovalItemDto> Items { get; init; } = Array.Empty<ProfileApprovalItemDto>();
    public bool HasAttachments { get; init; }
}
