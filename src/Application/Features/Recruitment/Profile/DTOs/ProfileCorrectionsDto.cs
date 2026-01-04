using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed record ProfileCorrectionsDto
{
    public UserProfileStatus Status { get; init; }
    public IReadOnlyList<CorrectionItemDto> Items { get; init; } = [];
}
