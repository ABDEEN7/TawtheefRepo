using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Configurations;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Services;

public class TokenService(IOptions<JwtSettings> jwtSettings, TimeProvider time, IUnitOfWork uow) : ITokenService
{
    private readonly SymmetricSecurityKey _securityKey = new(Encoding.UTF8.GetBytes(
        jwtSettings.Value.Key ?? throw new ArgumentException("Jwt:Key is missing in configuration")));

    public (string Token, DateTime Expires) GenerateAccessToken(User user, IEnumerable<Claim>? extraClaims = null)
    {
        if(user.UserType is null)
            throw new ArgumentException("User type is null. Cannot generate access token.");
        var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new("userType", user.UserType.BackendName),
            new(ClaimTypes.Role, user.UserType.BackendName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sid, (user.CurrentSessionId ?? Guid.Empty).ToString())
        };
        if (extraClaims is not null)
            claims.AddRange(extraClaims);
        
        var expires = time.GetLocalNow().DateTime.AddMinutes(
            jwtSettings.Value.ExpiryMinutes ?? 15);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Value.Issuer,
            audience: jwtSettings.Value.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        token.Header["alg"] = SecurityAlgorithms.HmacSha256;
        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
    
    public RefreshToken GenerateRefreshToken(Guid userId, string? ipAddress)
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires =  time.GetLocalNow().DateTime.AddDays(jwtSettings.Value.RefreshTokenExpirationDays ?? 7),
            CreatedDate = time.GetLocalNow().DateTime,
            UserId = userId,
            CreatedByIp = ipAddress,
        };
    }

    public async Task RevokeDescendantRefreshTokens(RefreshToken? refreshToken, string ipAddress, string reason)
    {
        if (!string.IsNullOrEmpty(refreshToken?.ReplacedByToken))
        {
            var childToken = await uow.GetEntityRepository<RefreshToken>().DbSet
                .FirstOrDefaultAsync(x => x.Token == refreshToken.ReplacedByToken);
            
            if (childToken is { IsActive: true })
            {
                await RevokeRefreshToken(childToken, ipAddress, reason);
            }
            else
            {
                await RevokeDescendantRefreshTokens(childToken, ipAddress, reason);
            }
        }
    }

    public async Task RevokeRefreshToken(RefreshToken token, string? ipAddress, string? reason = null, string? replacedByToken = null)
    {
        token.RevokedAt = time.GetLocalNow().DateTime;
        token.RevokedByIp = ipAddress;
        token.RevokedReason = reason;
        token.ReplacedByToken = replacedByToken;
        
        await uow.GetEntityRepository<RefreshToken>().UpdateAsync(token);
        await uow.SaveChangesAsync(cancellationToken: CancellationToken.None);
    }
}
