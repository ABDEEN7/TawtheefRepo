using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands
{
    public class RefreshTokenHandler(
        ILogger logger,
        UserManager<User> userManager,
        ITokenService tokenService,
        ISessionService sessions,
        IOptions<JwtSettings> jwtSettings)
        : ICommandHandler<RefreshTokenCommand, IResult<TokenResponse>>
    {
        public async Task<IResult<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(request.AccessToken))
                return Result.Fail<TokenResponse>(ErrorsCodes.AccessTokenRequired);
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Result.Fail<TokenResponse>(ErrorsCodes.RefreshTokenRequired);

            // Read principal from expired access token (no lifetime check)
            var principalResult = GetPrincipalFromExpiredToken(request.AccessToken);
            if (principalResult.IsFailed)
                return Result.Fail<TokenResponse>(principalResult.Errors);

            var principal = principalResult.Value;

            var nameIdentifier = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(nameIdentifier))
                return Result.Fail<TokenResponse>(ErrorsCodes.InvalidAccessToken);

            if (!Guid.TryParse(nameIdentifier, out var userId))
                return Result.Fail<TokenResponse>(ErrorsCodes.InvalidUserIdentifier);

            // Load user and their refresh tokens
            var user = await userManager.Users
                .Include(u => u.UserType)
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user is null)
                return Result.Fail<TokenResponse>(ErrorsCodes.UserNotFound);

            // Validate refresh token existence + activity
            var stored = user.RefreshTokens.FirstOrDefault(rt => rt.Token == request.RefreshToken);
            if (stored  is null)
                return Result.Fail<TokenResponse>(ErrorsCodes.RefreshTokenNotFound);

            if (!stored .IsActive)
                return Result.Fail<TokenResponse>(ErrorsCodes.InactiveRefreshToken);

            // Single-session checks using SecurityStamp

            var currentSid = await sessions.GetCurrentAsync(stored.UserId, cancellationToken);
            if (string.IsNullOrEmpty(currentSid) || !string.Equals(currentSid, stored.SecurityStamp, StringComparison.Ordinal))
                return Result.Fail<TokenResponse>(ErrorsCodes.SessionRevoked);

            var authResponseResult = await tokenService.IssueTokensAsync(user, "RefreshToken", cancellationToken);
            if (authResponseResult.IsFailed)
                return Result.Fail<TokenResponse>(authResponseResult.Errors);
            
            return Result.Ok(authResponseResult.Value.Token!);
        }

        private Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        {
            var settings = jwtSettings.Value;
            var parameters = settings.ToTokenValidationParameters();
            parameters.ValidateLifetime = false; // allow expired
            parameters.ClockSkew = TimeSpan.FromMinutes(1);
            
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, parameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwt)
                    return Result.Fail<ClaimsPrincipal>(ErrorsCodes.InvalidAccessToken);

                if (!jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
                    return Result.Fail<ClaimsPrincipal>(ErrorsCodes.InvalidAlgorithm);

                return principal;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to validate expired token");
                return Result.Fail<ClaimsPrincipal>(ErrorsCodes.InvalidAccessToken);
            }
        }
    }
}
