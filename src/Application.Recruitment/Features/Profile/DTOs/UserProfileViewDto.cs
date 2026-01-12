using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.DTOs;

public sealed record UserProfileViewDto
{
    public Guid UserProfileId { get; init; }
    public UserProfileStatus Status { get; init; }

    // Reuse your existing snapshot DTO (it’s fine)
    public ProfileApprovalDataDto Profile { get; init; } = default!;
}
