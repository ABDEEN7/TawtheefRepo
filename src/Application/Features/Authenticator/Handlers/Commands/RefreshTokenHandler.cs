using MediatR;
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
    : IRequestHandler<RefreshTokenCommand, IResult<TokenResponse>>
{
    public async Task<IResult<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var totalSw = System.Diagnostics.Stopwatch.StartNew();
        var timings = new Dictionary<string, long>();
        var metadata = new Dictionary<string, object> { ["IpAddress"] = request.IpAddress };

        try
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Result.Fail<TokenResponse>(UnauthorizedError(ErrorsCodes.RefreshTokenRequired));

            var incomingTokenHash = tokenService.HashRefreshToken(request.RefreshToken);

            var dbQuerySw = System.Diagnostics.Stopwatch.StartNew();
            var storedToken = await uow.GetEntityRepository<RefreshToken>().DbSet
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.TokenHash == incomingTokenHash, cancellationToken);
            timings["DbFetchStoredToken"] = dbQuerySw.ElapsedMilliseconds;

            if (storedToken is null)
                return Result.Fail<TokenResponse>(UnauthorizedError(ErrorsCodes.RefreshTokenNotFound));

            var user = storedToken.User;
            metadata["UserId"] = storedToken.UserId;
            metadata["Sid"] = storedToken.SecurityStamp;

            if (user is null)
            {
                return Result.Fail<TokenResponse>(UnauthorizedError(ErrorsCodes.UserNotFound));
            }

            var now = time.GetUtcNow().UtcDateTime;
            if (storedToken.IsRevoked)
            {
                await tokenService.RevokeSessionFamilyAsync(storedToken.UserId, storedToken.SecurityStamp,
                    request.IpAddress, "Refresh token replay detected", cancellationToken);
                return Result.Fail<TokenResponse>(ForbiddenError(ErrorsCodes.SessionRevoked));
            }

            if (storedToken.IsExpired)
            {
                storedToken.Revoke(now, request.IpAddress, "Refresh token expired");
                await uow.GetEntityRepository<RefreshToken>().UpdateAsync(storedToken);
                await uow.SaveChangesAsync(cancellationToken);
                return Result.Fail<TokenResponse>(UnauthorizedError(ErrorsCodes.InactiveRefreshToken));
            }

            var sessionCheckSw = System.Diagnostics.Stopwatch.StartNew();
            var isSessionActive = await tokenService.IsSessionActiveAsync(storedToken.UserId, storedToken.SecurityStamp, cancellationToken);
            timings["SessionActiveCheck"] = sessionCheckSw.ElapsedMilliseconds;

            if (!isSessionActive)
            {
                return Result.Fail<TokenResponse>(ForbiddenError(ErrorsCodes.SessionRevoked));
            }

            var rotationSw = System.Diagnostics.Stopwatch.StartNew();
            var authResponseResult = await tokenService.RotateRefreshTokenAsync(
                user,
                storedToken,
                request.IpAddress,
                cancellationToken);
            timings["TokenRotationTotal"] = rotationSw.ElapsedMilliseconds;

            if (authResponseResult.IsFailed)
                return Result.Fail<TokenResponse>(authResponseResult.Errors);

            return Result.Ok(authResponseResult.Value.Token!);
        }
        finally
        {
            totalSw.Stop();
            timings["TotalRequestDuration"] = totalSw.ElapsedMilliseconds;
            logger.Information("RefreshToken Performance Report: {@Timings}, {@Context}", timings, metadata);
        }
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

