using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Interfaces.Services.Security;

public interface ITokenService
{
    Task<IResult<AuthResponse>> IssueTokensAsync(User user, string loginSource, CancellationToken ct);
    Task RevokeAllAsync(Guid userId, CancellationToken ct);
}
