using System.Linq;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public record ProfileOverviewDto
{
    public Guid UserProfileId { get; init; }
    public UserProfileStatus Status { get; init; }
    public ProfileApprovalDataDto? ApprovedProfile { get; init; }
    public IReadOnlyList<ProfileOverviewSectionDto> Sections { get; init; } = Array.Empty<ProfileOverviewSectionDto>();
    public bool HasPendingChanges => Sections.Any(s => s.HasPendingChanges);
}

public record ProfileOverviewSectionDto
{
    public ProfileSection Section { get; init; }
    public IReadOnlyList<ProfileApprovalItemDto> PendingItems { get; init; } = Array.Empty<ProfileApprovalItemDto>();
    public bool HasPendingChanges => PendingItems.Any();
}
