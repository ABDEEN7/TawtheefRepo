using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.DTOs;

public sealed record UserProfileSummaryDto
{
    public Guid UserProfileId { get; init; }
    public UserProfileStatus Status { get; init; }
    public bool CanEdit { get; init; }
    public bool CanSubmit { get; init; }
}
