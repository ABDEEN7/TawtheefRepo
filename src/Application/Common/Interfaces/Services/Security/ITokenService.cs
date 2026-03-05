using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Interfaces.Services.Security;

public interface ITokenService
{
    Task<IResult<AuthResponse>> IssueTokensAsync(User user, string loginSource, CancellationToken ct);
    Task<IResult<AuthResponse>> RotateRefreshTokenAsync(User user, RefreshToken currentToken, string? ipAddress, CancellationToken ct);
    Task RevokeSessionFamilyAsync(Guid userId, string sessionId, string? ipAddress, string reason, CancellationToken ct);
    Task<string?> GetCurrentSessionIdAsync(Guid userId, CancellationToken ct);
    Task<bool> IsSessionActiveAsync(Guid userId, string sessionId, CancellationToken ct);
    string HashRefreshToken(string refreshToken);
    Task RevokeAllAsync(Guid userId, CancellationToken ct);
}
