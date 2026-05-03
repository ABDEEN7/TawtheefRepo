using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Data;
using Tawtheef.Infrastructure.Extensions;
using User = Tawtheef.Domain.Entities.Users.User;

namespace Tawtheef.Infrastructure.Services.Identity;

public class TokenService(
    IOptions<JwtSettings> jwtSettings,
    IOptions<AppConfigSettings> appSettings,
    UserManager<User> userManager,
    IProfileCompletenessService pcs,
    ISessionService sessions,
    IHttpContextAccessor httpContextAccessor,
    TimeProvider time,
    IUnitOfWork uow,
    ILoginAuditService loginAudit,
    IDistributedCache cache,
    IAppLogger logger,
    TawtheefDbContext dbContext) : ITokenService
{
    private readonly SymmetricSecurityKey _securityKey = new(Encoding.UTF8.GetBytes(
        jwtSettings.Value.SigningKey ?? throw new ArgumentException("Jwt:Key is missing in configuration")));
    private const string PermClaimType = "permission";
    private const string ProfileCompleteClaimType = "profile.completed";
    private const string UserTypeClaimType = "user_type";
    private const string LoginProviderClaimType = "login_provider";

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

        var refreshToken = GenerateRefreshToken(user.Id, sid, device?.Ip);
        user.RefreshTokens.Add(refreshToken);

        user.LastLoginDate = time.GetUtcNow().UtcDateTime;
        await userManager.UpdateSecurityStampAsync(user);
        await ClearUserCacheAsync(user.Id, ct);

        var tokenResult = await BuildAuthResponseAsync(user, refreshToken, sid, loginSource, ct);
        if (tokenResult.IsFailed)
            return tokenResult;

        // Set the current session in Redis ONLY after building the token response successfully.
        await sessions.SetCurrentAsync(user.Id, sid, device, ct);
        
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
        var swTotal = Stopwatch.StartNew();
        var timings = new Dictionary<string, long>();
        
        var swSetup = Stopwatch.StartNew();
        var now = time.GetUtcNow().UtcDateTime;

        // We generate a new session ID (sid) on every rotation. 
        // This ensures that:
        // 1. Refresh token rotation is more secure (session hijacking protection).
        // 2. We can "Force Refresh" by revoking all current sessions without killing the refresh tokens.
        var newSid = Guid.NewGuid().ToString("N");
        var replacement = GenerateRefreshToken(user.Id, newSid, ipAddress);
        timings["GenTokenObj"] = swSetup.ElapsedMilliseconds;

        var swRevoke = Stopwatch.StartNew();
        currentToken.Revoke(now, ipAddress, "Rotated");
        currentToken.ReplacedByToken = replacement;
        timings["RevokeLogic"] = swRevoke.ElapsedMilliseconds;

        var swAdd = Stopwatch.StartNew();
        // Optimization: Use direct DbSet operations to avoid virtual collection access (prevent lazy-load SELECT)
        dbContext.Set<RefreshToken>().Add(replacement);
        
        var entry = dbContext.Entry(currentToken);
        if (entry.State == EntityState.Detached)
        {
            dbContext.Attach(currentToken);
        }
        entry.State = EntityState.Modified;
        timings["AddAndAttach"] = swAdd.ElapsedMilliseconds;

        // Register the new session in the database/cache
        var device = BuildDeviceInfo(httpContextAccessor.HttpContext);
        await sessions.SetCurrentAsync(user.Id, newSid, device, ct);
        
        timings["UpdateSetupTotal"] = swSetup.ElapsedMilliseconds;

        var buildSw = Stopwatch.StartNew();
        await ClearUserCacheAsync(user.Id, ct);
        var loginProvider = (await userManager.GetLoginsAsync(user)).FirstOrDefault()?.ProviderDisplayName?.Replace(" ", "") ?? "Password";
        var result = await BuildAuthResponseAsync(user, replacement, newSid, loginProvider, ct);
        timings["BuildAuthResponse"] = buildSw.ElapsedMilliseconds;

        if (result.IsFailed)
        {
            logger.Information("RotateRefreshTokenAsync Performance Report (Failed): {@Timings}", timings);
            return result;
        }

        var saveSw = Stopwatch.StartNew();
        await uow.SaveChangesAsync(ct);
        timings["SaveChangesAsync"] = saveSw.ElapsedMilliseconds;
        
        timings["TotalRotation"] = swTotal.ElapsedMilliseconds;
        logger.Information("RotateRefreshTokenAsync Performance Report: {@Timings}", timings);
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
            await uow.GetEntityRepository<RefreshToken>().UpdateAsync(token, ct);
        }

        await uow.SaveChangesAsync(ct);
        await cache.RemoveAsync($"roles:{userId}", ct);
        await cache.RemoveAsync($"perms:{userId}", ct);
    }

    public async Task<string?> GetCurrentSessionIdAsync(Guid userId, CancellationToken ct)
    {
        return await sessions.GetCurrentAsync(userId, ct);
    }

    public async Task<bool> IsSessionActiveAsync(Guid userId, string sessionId, CancellationToken ct)
    {
        return await sessions.IsActiveAsync(userId, sessionId, ct);
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
        string loginSource,
        CancellationToken ct)
    {
        var timings = new Dictionary<string, long>();
        
        var dbSw = Stopwatch.StartNew();
        var userType = await uow.GetEntityRepository<UserType>().DbSet
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .FirstAsync(t => t.Id == user.UserTypeId, ct);
        timings["UserTypeQuery"] = dbSw.ElapsedMilliseconds;

        var profileComplete = true;
        var additionalClaims = new List<Claim>();
        ProfilePrefillDto? prefill = null;

        if (user is ApplicantUser applicantUser)
        {
            var pcsSw = Stopwatch.StartNew();
            prefill = !applicantUser.IsCompletedProfile ? await pcs.BuildPrefillAsync(user, ct) : null;
            timings["BuildPrefill"] = pcsSw.ElapsedMilliseconds;
            
            profileComplete = applicantUser.IsCompletedProfile;
            additionalClaims.Add(new Claim(ProfileCompleteClaimType, applicantUser.IsCompletedProfile ? "true" : "false"));
        }

        additionalClaims.Add(new Claim(LoginProviderClaimType, loginSource));

        var genSw = Stopwatch.StartNew();
        var accessToken = await GenerateAccessTokenAsync(user, userType, [
            new Claim(JwtRegisteredClaimNames.Sid, sid),
            ..additionalClaims
        ], ct);
        timings["GenerateAccessToken"] = genSw.ElapsedMilliseconds;

        var loginSw = Stopwatch.StartNew();
        var logins = await userManager.GetLoginsAsync(user);
        timings["GetLogins"] = loginSw.ElapsedMilliseconds;
        
        var providerName = logins.FirstOrDefault()?.ProviderDisplayName?.Replace(" ", "");

        logger.Information("BuildAuthResponse Performance Report: {@Timings}", timings);

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

        var rolesKey = $"roles:{user.Id}";
        var permsKey = $"perms:{user.Id}";

        var timings = new Dictionary<string, long>();
        
        var cacheReadSw = Stopwatch.StartNew();
        var cachedRoles = await cache.GetStringAsync(rolesKey, ct);
        var cachedPerms = await cache.GetStringAsync(permsKey, ct);
        timings["CacheRead"] = cacheReadSw.ElapsedMilliseconds;

        List<string> roles;
        IReadOnlyCollection<string> permissions;

        if (cachedRoles != null && cachedPerms != null)
        {
            roles = JsonSerializer.Deserialize<List<string>>(cachedRoles) ?? [];
            permissions = JsonSerializer.Deserialize<List<string>>(cachedPerms) ?? [];
        }
        else
        {
            var identitySw = Stopwatch.StartNew();
            
            // Bypass UserManager overhead by querying directly
            var userData = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == user.Id)
                .Select(u => new
                {
                    Roles = dbContext.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(dbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name!)
                        .ToList()
                })
                .FirstOrDefaultAsync(ct);

            roles = userData?.Roles ?? [];
            permissions = await GetUserPermissionsAsync(roles.AsReadOnly(), ct);
            
            timings["IdentityDbFetch"] = identitySw.ElapsedMilliseconds;

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(jwtSettings.Value.RefreshTokenExpirationHours ?? 3)
            };
            var cacheWriteSw = Stopwatch.StartNew();
            await cache.SetStringAsync(rolesKey, JsonSerializer.Serialize(roles), cacheOptions, ct);
            await cache.SetStringAsync(permsKey, JsonSerializer.Serialize(permissions), cacheOptions, ct);
            timings["CacheWrite"] = cacheWriteSw.ElapsedMilliseconds;
        }

        logger.Information("GenerateAccessToken Performance Report: {@Timings}", timings);

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
            Expires = time.GetUtcNow().UtcDateTime.AddHours(jwtSettings.Value.RefreshTokenExpirationHours ?? 3),
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
            await uow.GetEntityRepository<RefreshToken>().UpdateAsync(token, ct);
        }

        await uow.SaveChangesAsync(ct);
        await ClearUserCacheAsync(userId, ct);
    }

    public async Task ForceUserRefreshAsync(Guid userId, CancellationToken ct)
    {
        // Revoke all current sessions to invalidate current access tokens.
        await sessions.RevokeAllAsync(userId, ct);
        
        // Clear permissions cache so the next refresh fetches updated roles/permissions.
        await ClearUserCacheAsync(userId, ct);
        
        // We do NOT revoke refresh tokens here, allowing the user to get new tokens 
        // using their existing refresh token without re-logging in.
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

        var roleIds = await dbContext.Roles
            .AsNoTracking()
            .Where(r => roles.Contains(r.Name!))
            .Select(r => r.Id)
            .ToListAsync(ct);

        if (roleIds.Count == 0)
            return [];

        var perms = await dbContext.RoleClaims
            .AsNoTracking()
            .Where(rc => roleIds.Contains(rc.RoleId) && rc.ClaimType == PermClaimType)
            .Select(rc => rc.ClaimValue)
            .Where(v => v != null)
            .Distinct()
            .ToListAsync(ct);

        return perms!;
    }

    public async Task ClearUserCacheAsync(Guid userId, CancellationToken ct)
    {
        await cache.RemoveAsync($"roles:{userId}", ct);
        await cache.RemoveAsync($"perms:{userId}", ct);
    }
}
