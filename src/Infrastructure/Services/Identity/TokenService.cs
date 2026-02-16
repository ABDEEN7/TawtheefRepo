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
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Extensions;
using User = Tawtheef.Domain.Entities.Users.User;

namespace Tawtheef.Infrastructure.Services.Identity;

public class TokenService(
    IOptions<JwtSettings> jwtSettings,
    IOptions<AppConfigSettings> appSettings,
    UserManager<User> userManager,
    RoleManager<ApplicationRole> roleManager,
    IProfileCompletenessService pcs,
    ISessionService sessions,
    IHttpContextAccessor httpContextAccessor,
    TimeProvider time,
    IUnitOfWork uow,
    ILoginAuditService loginAudit) : ITokenService
{
    private readonly SymmetricSecurityKey _securityKey = new(Encoding.UTF8.GetBytes(
        jwtSettings.Value.SigningKey ?? throw new ArgumentException("Jwt:Key is missing in configuration")));
    private const string PermClaimType = "permission";
    private const string ProfileCompleteClaimType = "profile.complete";
    private const string UserTypeClaimType = "user_type";

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

        var refreshToken = GenerateRefreshToken(user.Id, sid, device?.Ip);
        user.RefreshTokens.Add(refreshToken);

        user.LastLoginDate = time.GetUtcNow().UtcDateTime;
        await userManager.UpdateSecurityStampAsync(user);

        var tokenResult = await BuildAuthResponseAsync(user, refreshToken, sid, ct);
        if (tokenResult.IsFailed)
            return tokenResult;

        await uow.SaveChangesAsync(ct);
        await loginAudit.LogAsync(new LoginAttemptEntry(user.Id, user.UserTypeId, loginSource,
            true, SessionId: sid, IpAddress: device?.Ip, AttemptedAt: time.GetUtcNow()), ct);

        return tokenResult;
    }

    public async Task<IResult<AuthResponse>> RotateRefreshTokenAsync(
        User user,
        RefreshToken currentToken,
        string? ipAddress,
        CancellationToken ct)
    {
        var now = time.GetUtcNow().UtcDateTime;
        var replacement = GenerateRefreshToken(user.Id, currentToken.SecurityStamp, ipAddress);

        currentToken.Revoke(now, ipAddress, "Rotated");
        currentToken.ReplacedByToken = replacement;

        user.RefreshTokens.Add(replacement);

        await uow.GetEntityRepository<RefreshToken>().UpdateAsync(currentToken);

        var result = await BuildAuthResponseAsync(user, replacement, currentToken.SecurityStamp, ct);
        if (result.IsFailed)
            return result;

        await uow.SaveChangesAsync(ct);
        return result;
    }

    public async Task RevokeSessionFamilyAsync(
        Guid userId,
        string sessionId,
        string? ipAddress,
        string reason,
        CancellationToken ct)
    {
        var now = time.GetUtcNow().UtcDateTime;

        await sessions.RevokeAsync(userId, sessionId, ct);

        var tokens = await uow.GetEntityRepository<RefreshToken>().DbSet
            .Where(x => x.UserId == userId && x.SecurityStamp == sessionId && x.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in tokens)
        {
            token.Revoke(now, ipAddress, reason);
            await uow.GetEntityRepository<RefreshToken>().UpdateAsync(token);
        }

        await uow.SaveChangesAsync(ct);
    }

    public async Task<string?> GetCurrentSessionIdAsync(Guid userId, CancellationToken ct)
    {
        return await sessions.GetCurrentAsync(userId, ct);
    }

    public string HashRefreshToken(string refreshToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(bytes);
    }

    private async Task<IResult<AuthResponse>> BuildAuthResponseAsync(
        User user,
        RefreshToken refreshToken,
        string sid,
        CancellationToken ct)
    {
        var userType = await uow.GetEntityRepository<UserType>().DbSet
            .AsNoTracking()
            .FirstAsync(t => t.Id == user.UserTypeId, ct);

        var profileComplete = true;
        var additionalClaims = new List<Claim>();
        ProfilePrefillDto? prefill = null;

        if (user is ApplicantUser applicantUser)
        {
            prefill = !applicantUser.IsCompletedProfile ? await pcs.BuildPrefillAsync(user, ct) : null;
            profileComplete = applicantUser.IsCompletedProfile;
            additionalClaims.Add(new Claim(ProfileCompleteClaimType, applicantUser.IsCompletedProfile ? "true" : "false"));
        }

        var accessToken = await GenerateAccessTokenAsync(user, userType, [
            new Claim(JwtRegisteredClaimNames.Sid, sid),
            ..additionalClaims
        ], ct);

        var logins = await userManager.GetLoginsAsync(user);
        var providerName = logins.FirstOrDefault()?.ProviderDisplayName?.Replace(" ", "");

        return Result.Ok(new AuthResponse(
            !profileComplete,
            new UserInfoResponse(user.Id, user.FullNameEn, user.Email!, user.Avatar, user.AgreedToTerms, providerName, prefill),
            new TokenResponse(
                accessToken.Token,
                DateTime.SpecifyKind(accessToken.Expires, DateTimeKind.Utc),
                refreshToken.TokenFingerprint,
                DateTime.SpecifyKind(refreshToken.Expires, DateTimeKind.Utc))
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
            new(UserTypeClaimType, userType.BackendName)
        };
        if (extraClaims is not null)
            claims.AddRange(extraClaims);
        claims.AddRange(roles.Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(permissions.Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(perm => new Claim(PermClaimType, perm)));

        claims = claims.GroupBy(c => (c.Type, c.Value)).Select(g => g.First()).ToList();

        var expires = time.GetUtcNow().UtcDateTime.AddMinutes(jwtSettings.Value.ExpiryMinutes ?? 15);

        var token = new JwtSecurityToken(
            issuer: appSettings.Value.BackendUrl,
            audience: appSettings.Value.FrontendUrl,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        token.Header["alg"] = SecurityAlgorithms.HmacSha256;
        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    private RefreshToken GenerateRefreshToken(Guid userId, string sid, string? ipAddress = null)
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return new RefreshToken
        {
            TokenHash = HashRefreshToken(rawToken),
            TokenFingerprint = rawToken,
            Expires = time.GetUtcNow().UtcDateTime.AddDays(jwtSettings.Value.RefreshTokenExpirationDays ?? 7),
            CreatedDate = time.GetUtcNow().UtcDateTime,
            UserId = userId,
            CreatedByIp = ipAddress,
            SecurityStamp = sid
        };
    }

    public async Task RevokeAllAsync(Guid userId, CancellationToken ct)
    {
        await sessions.RevokeAllAsync(userId, ct);

        var now = time.GetUtcNow().UtcDateTime;
        var refreshTokens = await uow.GetEntityRepository<RefreshToken>().DbSet
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in refreshTokens)
        {
            token.Revoke(now, null, "Admin/explicit revoke-all");
            await uow.GetEntityRepository<RefreshToken>().UpdateAsync(token);
        }

        await uow.SaveChangesAsync(ct);
    }

    private static DeviceInfo? BuildDeviceInfo(HttpContext? ctx)
    {
        if (ctx is null) return null;
        var ip = ctx.Connection.RemoteIpAddress?.ToString();
        ctx.Request.Headers.TryGetValue("User-Agent", out var ua);
        var platform = ctx.Request.Headers.TryGetValue("X-Platform", out var p) ? p.ToString() : null;
        var version = ctx.Request.Headers.TryGetValue("X-App-Version", out var v) ? v.ToString() : null;
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
            return [];

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
