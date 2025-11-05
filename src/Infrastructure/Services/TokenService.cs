using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Services;

public class TokenService(IOptions<JwtSettings> jwtSettings, 
    UserManager<User> userManager,
    IProfileCompletenessService pcs,
    TimeProvider time, IUnitOfWork uow) : ITokenService
{
    private readonly SymmetricSecurityKey _securityKey = new(Encoding.UTF8.GetBytes(
        jwtSettings.Value.Key ?? throw new ArgumentException("Jwt:Key is missing in configuration")));
    public async Task<Result<AuthResponse>> IssueTokensAsync(User user, CancellationToken ct)
    {
        user.UserType = await uow.GetEntityRepository<UserType>().DbSet
            .FirstAsync(t => t.Id == user.UserTypeId, ct);

        await RevokeAllAsync(user.Id, ct);
        await userManager.UpdateSecurityStampAsync(user);
        var securityStamp = await userManager.GetSecurityStampAsync(user);

        var (isComplete, missing) = await pcs.EvaluateAsync(user.Id, ct);
        var accessToken =
            GenerateAccessToken(user, [
                new Claim(JwtRegisteredClaimNames.Sid, securityStamp),
                new("profile.completed", isComplete ? "true" : "false"),
                new("profile.missing.count", missing.Length.ToString())
            ]);
        var refreshToken = GenerateRefreshToken(user.Id, securityStamp);
        
        var prefill = await pcs.BuildPrefillAsync(user, ct);
        return Result.Success(new AuthResponse(
            !isComplete,
            new UserInfoResponse(user.Id, user.GivenNameEn, user.FamilyNameEn, user.Email!, user.Avatar),
            new TokenResponse(accessToken.Token, accessToken.Expires, refreshToken.Token, refreshToken.Expires),
            missing,
            prefill
        ));
    }
    
    private (string Token, DateTime Expires) GenerateAccessToken(User user, IEnumerable<Claim>? extraClaims = null)
    {
        if(user.UserType is null)
            throw new ArgumentException("User type is null. Cannot generate access token.");
        var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(nameof(user.UserType), user.UserType.BackendName),
            new(ClaimTypes.Role, user.UserType.BackendName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sid, user.SecurityStamp ?? string.Empty)
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
    
    private RefreshToken GenerateRefreshToken(Guid userId, string sid, string? ipAddress = null)
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires =  time.GetLocalNow().DateTime.AddDays(jwtSettings.Value.RefreshTokenExpirationDays ?? 7),
            CreatedDate = time.GetLocalNow().DateTime,
            UserId = userId,
            CreatedByIp = ipAddress,
            SecurityStamp = sid   
        };
    }

    private async Task RevokeDescendantRefreshTokens(RefreshToken? refreshToken, string ipAddress, string reason)
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

    private async Task RevokeRefreshToken(RefreshToken token, string? ipAddress, string? reason = null, string? replacedByToken = null)
    {
        token.RevokedAt = time.GetLocalNow().DateTime;
        token.RevokedByIp = ipAddress;
        token.RevokedReason = reason;
        token.ReplacedByToken = replacedByToken;
        
        await uow.GetEntityRepository<RefreshToken>().UpdateAsync(token);
        await uow.SaveChangesAsync(cancellationToken: CancellationToken.None);
    }
    
    public async Task RevokeAllAsync(Guid userId, CancellationToken ct)
    {
        var refreshTokens = await uow.GetEntityRepository<RefreshToken>().DbSet
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ToListAsync(ct);
        
        foreach (var token in refreshTokens)
        {
            await RevokeRefreshToken(token, null, "User logged out");
        }
    }
}
