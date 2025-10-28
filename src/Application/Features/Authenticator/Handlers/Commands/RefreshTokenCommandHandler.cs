using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class RefreshTokenCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService,
    IOptions<JwtSettings> jwtSettings)
    : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Validate input tokens
        if (string.IsNullOrEmpty(request.AccessToken))
            return Result.Failure<TokenResponse>(ErrorsCodes.AccessTokenRequired);
        if (string.IsNullOrEmpty(request.RefreshToken))
            return Result .Failure<TokenResponse>(ErrorsCodes.RefreshTokenRequired);

        // Get principal from an expired access token
        var principalResult = GetPrincipalFromExpiredToken(request.AccessToken);
        if(principalResult.IsFailure)
            return Result .Failure<TokenResponse>(principalResult.Error);
        
        var principal = principalResult.Value;
        var nameIdentifier = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if( string.IsNullOrEmpty(nameIdentifier))
            return Result .Failure<TokenResponse>(ErrorsCodes.InvalidAccessToken);

        if (!Guid.TryParse(nameIdentifier, out var userId))
            return Result .Failure<TokenResponse>(ErrorsCodes.InvalidUserIdentifier);

        // Fetch user with refresh tokens
        var user = await userManager.Users
            .Include(u => u.UserType)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
            return Result .Failure<TokenResponse>(ErrorsCodes.UserNotFound);

        if (user.CurrentSessionId is null)
            return Result.Failure<TokenResponse>(ErrorsCodes.SessionExpired);
        
        // Validate refresh token
        var refreshToken = user.RefreshTokens
            .FirstOrDefault(rt => rt.Token == request.RefreshToken);
        if (refreshToken == null)
            return Result .Failure<TokenResponse>(ErrorsCodes.RefreshTokenNotFound);

        if (!refreshToken.IsActive)
            return Result .Failure<TokenResponse>(ErrorsCodes.InactiveRefreshToken);

        // Generate new tokens
        var newRefreshToken = tokenService.GenerateRefreshToken(user.Id, request.IpAddress);
        await tokenService.RevokeRefreshToken(refreshToken, request.IpAddress, "Replaced by new token", newRefreshToken.Token);

        // Update user tokens
        user.AddRefreshToken(newRefreshToken.Token, newRefreshToken.Expires, request.IpAddress);
        user.RemoveOldRefreshTokens(jwtSettings.Value.RefreshTokenRetentionCount ?? 5);

        await userManager.UpdateAsync(user);

        // Generate a new access token
        var accessToken = tokenService.GenerateAccessToken(user);

        return Result.Success(new TokenResponse(
            accessToken.Token,
            accessToken.Expires,
            newRefreshToken.Token,
            newRefreshToken.Expires
        ));
    }

    private Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidAudience = jwtSettings.Value.Audience,
            ValidIssuer = jwtSettings.Value.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Key)),
            ValidateLifetime = false, // Allow expired tokens
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
    
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        
            if (securityToken is not JwtSecurityToken jwtSecurityToken)
                return Result.Failure<ClaimsPrincipal>(ErrorsCodes.InvalidAccessToken);

            // Check for both algorithm formats
            var isAlgorithmValid = 
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase) ||
                jwtSecurityToken.Header.Alg.Equals("http://www.w3.org/2001/04/xmldsig-more#hmac-sha256", StringComparison.InvariantCultureIgnoreCase);

            return !isAlgorithmValid ? Result.Failure<ClaimsPrincipal>(ErrorsCodes.InvalidAlgorithm) : principal;
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
