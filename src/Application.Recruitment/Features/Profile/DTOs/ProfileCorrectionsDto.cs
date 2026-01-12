using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.DTOs;

public sealed record ProfileCorrectionsDto
{
    public UserProfileStatus Status { get; init; }
    public IReadOnlyList<CorrectionItemDto> Items { get; init; } = [];
}
