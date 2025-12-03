using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FluentResults;
using Microsoft.AspNetCore.Http;
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

namespace Tawtheef.Infrastructure.Services.Identity;

public class TokenService(IOptions<JwtSettings> jwtSettings, 
    UserManager<User> userManager,
    IProfileCompletenessService pcs,
    ISessionService sessions,              
    IHttpContextAccessor httpContextAccessor,
    TimeProvider time, IUnitOfWork uow) : ITokenService
{
    private readonly SymmetricSecurityKey _securityKey = new(Encoding.UTF8.GetBytes(
        jwtSettings.Value.SigningKey ?? throw new ArgumentException("Jwt:Key is missing in configuration")));
    public async Task<IResult<AuthResponse>> IssueTokensAsync(User user, CancellationToken ct)
    {
        var sid = Guid.NewGuid().ToString("N");
        var device = BuildDeviceInfo(httpContextAccessor.HttpContext);
        await sessions.SetCurrentAsync(user.Id, sid, device, ct);
        var refreshToken = GenerateRefreshToken(user.Id, sid);
        user.RefreshTokens.Add(refreshToken);
        user.LastLoginDate = time.GetUtcNow().DateTime;
        await userManager.UpdateSecurityStampAsync(user);

        var userType = await uow.GetEntityRepository<UserType>().DbSet
            .AsNoTracking().FirstAsync(t => t.Id == user.UserTypeId, ct);
        var data = await pcs.EvaluateAsync(user.Id, ct);
        var prefill = !data.IsComplete ? await pcs.BuildPrefillAsync(user, ct) : null;
        var accessToken =
            GenerateAccessToken(user, userType, [
                new Claim(JwtRegisteredClaimNames.Sid, sid),
                new("profile.completed", data.IsComplete ? "true" : "false")
            ]);
        
        return Result.Ok(new AuthResponse(
            !data.IsComplete,
            new UserInfoResponse(user.Id, user.FullNameEn, user.Email!, user.Avatar, prefill),
            new TokenResponse(accessToken.Token, accessToken.Expires, refreshToken.Token, refreshToken.Expires)
        ));
    }
    
    private (string Token, DateTime Expires) GenerateAccessToken(User user, UserType userType, IEnumerable<Claim>? extraClaims = null)
    {
        var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(nameof(user.UserType), userType.BackendName),
            new(ClaimTypes.Role, userType.BackendName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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
            SecurityStamp = sid // reuse column to store session id
        };
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
        // Clear session (so access tokens fail sid check on next request)
        await sessions.RevokeAllAsync(userId, ct);

        // Revoke any active refresh tokens
        var refreshTokens = await uow.GetEntityRepository<RefreshToken>().DbSet
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in refreshTokens)
            await RevokeRefreshToken(token, null, "Admin/explicit revoke-all");
    }
    private static DeviceInfo? BuildDeviceInfo(HttpContext? ctx)
    {
        if (ctx is null) return null;
        var ip = ctx.Connection.RemoteIpAddress?.ToString();
        ctx.Request.Headers.TryGetValue("User-Agent", out var ua);
        var platform = ctx.Request.Headers.TryGetValue("X-Platform", out var p) ? p.ToString() : null;
        var version  = ctx.Request.Headers.TryGetValue("X-App-Version", out var v) ? v.ToString() : null;
        return new DeviceInfo(ip, ua.ToString(), platform, version);
    }
}
