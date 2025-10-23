using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Configurations;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class GenerateLinkTokenCommandHandler(IOptions<JwtSettings> jwtSettings, TimeProvider time)
    : IRequestHandler<GenerateLinkTokenCommand, string>
{
    public Task<string> Handle(GenerateLinkTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSettings.Value.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim("userId", request.UserId.ToString()),
                new Claim("tokenType", "oauth_link")
            ]),
            // Use DateTime.UtcNow directly or keep it as DateTimeOffset
            Expires = time.GetLocalNow().DateTime.AddMinutes(jwtSettings.Value.TokenLifetimeMinutes), // Short-lived token
            NotBefore = time.GetLocalNow().DateTime, // Explicitly set NotBefore
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Task.FromResult(tokenHandler.WriteToken(token));
    }
}
