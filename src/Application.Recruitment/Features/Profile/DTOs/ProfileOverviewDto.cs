using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.DTOs;

public record ProfileOverviewDto
{
    public Guid UserProfileId { get; init; }
    public UserProfileStatus Status { get; init; }
    public ProfileApprovalDataDto? ApprovedProfile { get; init; }
    public ProfileRequestProgressDto RequestProgress { get; init; } = new();
    public IReadOnlyList<ProfileOverviewSectionDto> Sections { get; init; } = [];
    public bool HasPendingChanges => Sections.Any(s => s.HasPendingChanges);
}
