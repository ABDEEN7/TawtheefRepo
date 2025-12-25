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
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Extensions;
using User = Tawtheef.Domain.Entities.Users.User;

namespace Tawtheef.Infrastructure.Services.Identity;

public class TokenService(IOptions<JwtSettings> jwtSettings,
    UserManager<User> userManager,
    RoleManager<ApplicationRole> roleManager,
    IProfileCompletenessService pcs,
    ISessionService sessions,
    IHttpContextAccessor httpContextAccessor,
    TimeProvider time, IUnitOfWork uow,
    ILoginAuditService loginAudit) : ITokenService
{
    private readonly SymmetricSecurityKey _securityKey = new(Encoding.UTF8.GetBytes(
        jwtSettings.Value.SigningKey ?? throw new ArgumentException("Jwt:Key is missing in configuration")));
    private const string PermClaimType = "permission";
    private const string ProfileCompleteClaimType = "profile.complete";
    
    
    
    public async Task<IResult<AuthResponse>> IssueTokensAsync(User user, string loginSource, CancellationToken ct)
    {
        if (user.IsBlocked)
        {
            await loginAudit.LogAsync(
                new LoginAttemptEntry(user.Id, user.UserTypeId, loginSource, false,
                    ErrorsCodes.AccountStatusNotAllowedForLogin,
                    IpAddress: httpContextAccessor.HttpContext?.GetClientIpAddress()), ct);
            return Result.Fail<AuthResponse>(ErrorsCodes.AccountStatusNotAllowedForLogin);
        }

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
            await GenerateAccessTokenAsync(user, userType, [
                new Claim(JwtRegisteredClaimNames.Sid, sid),
                new Claim(ProfileCompleteClaimType, data.IsComplete ? "true" : "false")
            ], ct);

        await loginAudit.LogAsync(new LoginAttemptEntry(user.Id, user.UserTypeId,  loginSource,
            true, SessionId: sid, IpAddress: device?.Ip, AttemptedAt: time.GetUtcNow()), ct);
        
        var logins = await userManager.GetLoginsAsync(user);
        var providerName = logins.FirstOrDefault()?.ProviderDisplayName?.Replace(" ", "");
        return Result.Ok(new AuthResponse(
            !data.IsComplete,
            new UserInfoResponse(user.Id, user.FullNameEn, user.Email!, user.Avatar, providerName,prefill),
            new TokenResponse(accessToken.Token, accessToken.Expires, refreshToken.Token, refreshToken.Expires)
        ));
    }
    
    private async Task<(string Token, DateTime Expires)> GenerateAccessTokenAsync(User user, UserType userType,
        IEnumerable<Claim>? extraClaims = null, CancellationToken ct = default)
    {
        var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

        var roles = await userManager.GetRolesAsync(user);
        var permissions = await GetUserPermissionsAsync(roles.AsReadOnly(), ct);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(nameof(user.UserType), userType.BackendName)
        };
        if (extraClaims is not null)
            claims.AddRange(extraClaims);
        claims.AddRange(roles.Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(permissions.Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(perm => new Claim(PermClaimType, perm)));

        // Remove duplicate claims
        claims = claims.GroupBy(c => (c.Type, c.Value))
            .Select(g => g.First()).ToList();
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
    
    private async Task<IReadOnlyCollection<string>> GetUserPermissionsAsync(
        IReadOnlyCollection<string> roleNames,
        CancellationToken ct)
    {
        var roles = roleNames
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (roles.Count == 0)
            return Array.Empty<string>();

        // Fetch roles (no need for custom tables)
        var roleEntities = await roleManager.Roles
            .AsNoTracking()
            .Where(r => r.Name != null && roles.Contains(r.Name))
            .ToListAsync(ct);

        var perms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var role in roleEntities)
        {
            var claims = await roleManager.GetClaimsAsync(role);
            foreach (var c in claims.Where(x => x.Type == PermClaimType))
                perms.Add(c.Value);
        }

        return perms.ToList();
    }
}
