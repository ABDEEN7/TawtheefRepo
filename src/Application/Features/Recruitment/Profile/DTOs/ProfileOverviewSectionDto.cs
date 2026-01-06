using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public record ProfileOverviewSectionDto
{
    public ProfileSection Section { get; init; }
    public IReadOnlyList<ProfileApprovalItemDto> PendingItems { get; init; } = [];
    public bool HasPendingChanges => PendingItems.Any();
}
