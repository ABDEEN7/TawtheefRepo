using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface ITokenService
{
    Task<IResult<AuthResponse>> IssueTokensAsync(User user, CancellationToken ct);
    Task RevokeAllAsync(Guid userId, CancellationToken ct);
}
