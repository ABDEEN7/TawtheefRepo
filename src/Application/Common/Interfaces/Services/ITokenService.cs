using System;
using System.Threading.Tasks;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface ITokenService
{
    (string Token, DateTime Expires) GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(Guid userId, string? ipAddress = null);
    Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress, string reason);
    Task RevokeRefreshToken(RefreshToken token, string? ipAddress, string? reason = null, string? replacedByToken = null);
}
