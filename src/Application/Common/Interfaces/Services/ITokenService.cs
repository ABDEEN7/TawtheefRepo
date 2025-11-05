using CSharpFunctionalExtensions;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface ITokenService
{
    Task<Result<AuthResponse>> IssueTokensAsync(User user, CancellationToken ct);
    Task RevokeAllAsync(Guid userId, CancellationToken ct);
}
