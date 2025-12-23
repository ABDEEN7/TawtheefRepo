using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed record CorrectionItemDto
{
    public ProfileSection Section { get; init; }
    public string Note { get; init; } = string.Empty;
}