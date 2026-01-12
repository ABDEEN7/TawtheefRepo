using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Features.Profile.DTOs;

public record ProfileOverviewSectionDto
{
    public ProfileSection Section { get; init; }
    public IReadOnlyList<ProfileApprovalItemDto> PendingItems { get; init; } = [];
    public bool HasPendingChanges => PendingItems.Any();
}
