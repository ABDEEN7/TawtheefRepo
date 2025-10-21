using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Configurations;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class ValidateLinkTokenCommandHandler(IOptions<JwtSettings> jwtSettings)
    : IRequestHandler<ValidateLinkTokenCommand, Result<LinkTokenValidationResult>>
{

    public Task<Result<LinkTokenValidationResult>> Handle(ValidateLinkTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Value.Key);

            tokenHandler.ValidateToken(request.Token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            // Verify token type
            var tokenType = jwtToken.Claims.FirstOrDefault(x => x.Type == "tokenType")?.Value;
            if (tokenType != "oauth_link")
                return Task.FromResult(Result.Failure<LinkTokenValidationResult>(ErrorsCodes.InvalidTokenType));

            // Get user ID
            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "userId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Task.FromResult(Result.Failure<LinkTokenValidationResult>(ErrorsCodes.InvalidUserIdentifier));
            }

            return Task.FromResult<Result<LinkTokenValidationResult>>(new LinkTokenValidationResult(userId, jwtToken.ValidTo));
        }
        catch (SecurityTokenExpiredException)
        {
            return Task.FromResult(Result.Failure<LinkTokenValidationResult>(ErrorsCodes.TokenExpired));
        }
        catch (Exception)
        {
            return Task.FromResult(Result.Failure<LinkTokenValidationResult>(ErrorsCodes.InvalidToken));
        }
    }
}
