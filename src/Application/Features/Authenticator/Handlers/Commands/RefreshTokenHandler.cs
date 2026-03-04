using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Auth;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class RefreshTokenHandler(
    IAppLogger logger,
    ITokenService tokenService,
    IUnitOfWork uow,
    TimeProvider time)
    : ICommandHandler<RefreshTokenCommand, IResult<TokenResponse>>
{
    public async Task<IResult<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return Result.Fail<TokenResponse>(UnauthorizedError(ErrorsCodes.RefreshTokenRequired));

        var incomingTokenHash = tokenService.HashRefreshToken(request.RefreshToken);
        var storedToken = await uow.GetEntityRepository<RefreshToken>().DbSet
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == incomingTokenHash, cancellationToken);

        if (storedToken is null)
            return Result.Fail<TokenResponse>(UnauthorizedError(ErrorsCodes.RefreshTokenNotFound));

        var user = storedToken.User;
        if (user is null)
        {
            logger.Warning("Refresh token has no associated user", new { storedToken.Id, storedToken.UserId });
            return Result.Fail<TokenResponse>(UnauthorizedError(ErrorsCodes.UserNotFound));
        }

        var now = time.GetUtcNow().UtcDateTime;
        if (storedToken.IsRevoked)
        {
            await tokenService.RevokeSessionFamilyAsync(storedToken.UserId, storedToken.SecurityStamp,
                request.IpAddress, "Refresh token replay detected", cancellationToken);
            logger.Warning("Refresh token reuse detected", new { storedToken.UserId, storedToken.SecurityStamp });
            return Result.Fail<TokenResponse>(ForbiddenError(ErrorsCodes.SessionRevoked));
        }

        if (storedToken.IsExpired)
        {
            storedToken.Revoke(now, request.IpAddress, "Refresh token expired");
            await uow.GetEntityRepository<RefreshToken>().UpdateAsync(storedToken);
            await uow.SaveChangesAsync(cancellationToken);
            return Result.Fail<TokenResponse>(UnauthorizedError(ErrorsCodes.InactiveRefreshToken));
        }

        var currentSid = await tokenService.GetCurrentSessionIdAsync(storedToken.UserId, cancellationToken);
        if (string.IsNullOrEmpty(currentSid) || !string.Equals(currentSid, storedToken.SecurityStamp, StringComparison.Ordinal))
        {
            logger.Warning("Session revoked for user {UserId}. Stored Token SID: {StoredSid}, Current Active SID: {CurrentSid}", 
                storedToken.UserId, storedToken.SecurityStamp, currentSid);
            return Result.Fail<TokenResponse>(ForbiddenError(ErrorsCodes.SessionRevoked));
        }

        var authResponseResult = await tokenService.RotateRefreshTokenAsync(
            user,
            storedToken,
            request.IpAddress,
            cancellationToken);

        if (authResponseResult.IsFailed)
            return Result.Fail<TokenResponse>(authResponseResult.Errors);

        return Result.Ok(authResponseResult.Value.Token!);
    }

    private static Error UnauthorizedError(string errorCode)
    {
        return new Error("Unauthorized")
            .WithMetadata("Code", errorCode)
            .WithMetadata("StatusCode", StatusCodes.Status401Unauthorized);
    }

    private static Error ForbiddenError(string errorCode)
    {
        return new Error("Forbidden")
            .WithMetadata("Code", errorCode)
            .WithMetadata("StatusCode", StatusCodes.Status403Forbidden);
    }
}
