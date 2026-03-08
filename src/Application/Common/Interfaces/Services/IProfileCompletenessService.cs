using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface IProfileCompletenessService
{
    Task<ProfileStatusDto> EvaluateAsync(Guid userId, CancellationToken ct, User? user = null);
    Task<ProfilePrefillDto> BuildPrefillAsync(User user, CancellationToken ct);
}
