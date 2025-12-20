using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;

public record ProfileApprovalSectionDto
{
    public ProfileSection Section { get; init; }
    public ProfileApprovalItemDto? SectionReview { get; init; }
    public bool HasAttachments { get; init; }
}
