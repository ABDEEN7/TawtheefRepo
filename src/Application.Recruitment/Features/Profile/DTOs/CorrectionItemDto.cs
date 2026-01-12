using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Features.Profile.DTOs;

public sealed record CorrectionItemDto
{
    public ProfileSection Section { get; init; }
    public string Note { get; init; } = string.Empty;
}
