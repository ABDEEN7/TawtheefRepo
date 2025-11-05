using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface IProfileCompletenessService
{
    Task<(bool isComplete, string[] missing)> EvaluateAsync(Guid userId, CancellationToken ct);
    Task<ProfilePrefillDto?> BuildPrefillAsync(User user, CancellationToken ct);
}
