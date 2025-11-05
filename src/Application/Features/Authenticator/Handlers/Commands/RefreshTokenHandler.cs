using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Configurations;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands
{
    public class RefreshTokenHandler(
        UserManager<User> userManager,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings)
        : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
    {
        public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(request.AccessToken))
                return Result.Failure<TokenResponse>(ErrorsCodes.AccessTokenRequired);
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Result.Failure<TokenResponse>(ErrorsCodes.RefreshTokenRequired);

            // Read principal from expired access token (no lifetime check)
            var principalResult = GetPrincipalFromExpiredToken(request.AccessToken);
            if (principalResult.IsFailure)
                return Result.Failure<TokenResponse>(principalResult.Error);

            var principal = principalResult.Value;

            var nameIdentifier = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(nameIdentifier))
                return Result.Failure<TokenResponse>(ErrorsCodes.InvalidAccessToken);

            if (!Guid.TryParse(nameIdentifier, out var userId))
                return Result.Failure<TokenResponse>(ErrorsCodes.InvalidUserIdentifier);

            // Load user and their refresh tokens
            var user = await userManager.Users
                .Include(u => u.UserType)
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user is null)
                return Result.Failure<TokenResponse>(ErrorsCodes.UserNotFound);

            // Validate refresh token existence + activity
            var refreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == request.RefreshToken);
            if (refreshToken is null)
                return Result.Failure<TokenResponse>(ErrorsCodes.RefreshTokenNotFound);

            if (!refreshToken.IsActive)
                return Result.Failure<TokenResponse>(ErrorsCodes.InactiveRefreshToken);

            // Single-session checks using SecurityStamp

            // Access token SID must match the refresh token’s stored stamp (prevents mixing)
            var sidFromAccess =
                principal.FindFirstValue(JwtRegisteredClaimNames.Sid) ??
                principal.FindFirstValue("sid"); // some libs emit "sid" short name

            if (string.IsNullOrWhiteSpace(sidFromAccess))
                return Result.Failure<TokenResponse>(ErrorsCodes.InvalidAccessToken);

            if (!string.Equals(sidFromAccess, refreshToken.SecurityStamp, StringComparison.Ordinal))
                return Result.Failure<TokenResponse>(ErrorsCodes.SessionRevoked);

            var authResponseResult = await tokenService.IssueTokensAsync(user, cancellationToken);
            if (authResponseResult.IsFailure)
                return Result.Failure<TokenResponse>(authResponseResult.Error);
            
            return Result.Success(authResponseResult.Value.Token!);
        }

        private Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        {
            var settings = jwtSettings.Value;
            if (string.IsNullOrWhiteSpace(settings.Key))
                return Result.Failure<ClaimsPrincipal>("JWT key is missing");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = settings.Audience,
                ValidIssuer = settings.Issuer,
                ValidateIssuer = true,           // ensure issuer matches
                ValidateAudience = true,         // ensure audience matches
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                ValidateLifetime = false,        // allow expired tokens (we’re just reading claims)
                ClockSkew = TimeSpan.FromMinutes(1)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken)
                    return Result.Failure<ClaimsPrincipal>(ErrorsCodes.InvalidAccessToken);

                // Accept common alg ids
                var alg = jwtSecurityToken.Header.Alg;
                var isAlgorithmValid =
                    alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase) ||
                    alg.Equals("http://www.w3.org/2001/04/xmldsig-more#hmac-sha256", StringComparison.OrdinalIgnoreCase);

                return !isAlgorithmValid
                    ? Result.Failure<ClaimsPrincipal>(ErrorsCodes.InvalidAlgorithm)
                    : principal;
            }
            catch (SecurityTokenException ex)
            {
                return Result.Failure<ClaimsPrincipal>($"Token validation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result.Failure<ClaimsPrincipal>($"An error occurred while validating the token: {ex.Message}");
            }
        }
    }
}
