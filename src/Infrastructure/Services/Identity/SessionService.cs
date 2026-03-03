using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Services.Identity;

public sealed class EfSessionService(
    IUnitOfWork uow, 
    IDistributedCache cache,
    IOptions<JwtSettings> jwtSettings) : ISessionService
{
    private static string GetSidKey(Guid userId) => $"sid:{userId}";

    public async Task SetCurrentAsync(Guid userId, string sessionId, DeviceInfo? device, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<UserSession>();
        // Revoke all active sessions for this user
        await repo.DbSet
            .Where(s => s.UserId == userId && s.RevokedAtUtc == null)
            .ExecuteUpdateAsync(u => u.SetProperty(s => s.RevokedAtUtc, _ => DateTime.UtcNow), ct);

        // Add the new one
        await repo.AddAsync(new UserSession {
            UserId = userId,
            SessionId = sessionId,
            CreatedAtUtc = DateTime.UtcNow,
            Ip = device?.Ip,
            UserAgent = device?.UserAgent,
            Platform = device?.Platform,
            AppVersion = device?.AppVersion
        });

        await uow.SaveChangesAsync(ct);

        // Update Cache
        await cache.SetStringAsync(GetSidKey(userId), sessionId, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(jwtSettings.Value.RefreshTokenExpirationHours ?? 3)
        }, ct);
    }

    public async Task<string?> GetCurrentAsync(Guid userId, CancellationToken ct)
    {
        var key = GetSidKey(userId);
        var sid = await cache.GetStringAsync(key, ct);
        if (sid != null) return sid;

        // The latest non-revoked (there should be at most one)
        sid = await uow.GetEntityRepository<UserSession>().DbSet
            .Where(s => s.UserId == userId && s.RevokedAtUtc == null)
            .OrderByDescending(s => s.CreatedAtUtc)
            .Select(s => s.SessionId)
            .FirstOrDefaultAsync(ct);

        if (sid != null)
        {
            await cache.SetStringAsync(key, sid, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(jwtSettings.Value.RefreshTokenExpirationHours ?? 3)
            }, ct);
        }

        return sid;
    }

    public async Task RevokeAllAsync(Guid userId, CancellationToken ct)
    {
        await uow.GetEntityRepository<UserSession>().DbSet
            .Where(s => s.UserId == userId && s.RevokedAtUtc == null)
            .ExecuteUpdateAsync(u => u.SetProperty(s => s.RevokedAtUtc, _ => DateTime.UtcNow), ct);

        await cache.RemoveAsync(GetSidKey(userId), ct);
    }

    public async Task RevokeAsync(Guid userId, string sessionId, CancellationToken ct)
    {
        await uow.GetEntityRepository<UserSession>().DbSet
            .Where(s => s.UserId == userId && s.SessionId == sessionId && s.RevokedAtUtc == null)
            .ExecuteUpdateAsync(u => u.SetProperty(s => s.RevokedAtUtc, _ => DateTime.UtcNow), ct);

        // Invalidate cache if it matches the revoked session
        var current = await cache.GetStringAsync(GetSidKey(userId), ct);
        if (current == sessionId)
        {
            await cache.RemoveAsync(GetSidKey(userId), ct);
        }
    }
}
