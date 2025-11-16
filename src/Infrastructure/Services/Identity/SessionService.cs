using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Services.Identity;

public sealed class EfSessionService(IUnitOfWork uow) : ISessionService
{
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
    }

    public async Task<string?> GetCurrentAsync(Guid userId, CancellationToken ct)
    {
        // The latest non-revoked (there should be at most one)
        var sid = await uow.GetEntityRepository<UserSession>().DbSet
            .Where(s => s.UserId == userId && s.RevokedAtUtc == null)
            .OrderByDescending(s => s.CreatedAtUtc)
            .Select(s => s.SessionId)
            .FirstOrDefaultAsync(ct);

        return sid;
    }

    public async Task RevokeAllAsync(Guid userId, CancellationToken ct)
    {
        await uow.GetEntityRepository<UserSession>().DbSet
            .Where(s => s.UserId == userId && s.RevokedAtUtc == null)
            .ExecuteUpdateAsync(u => u.SetProperty(s => s.RevokedAtUtc, _ => DateTime.UtcNow), ct);
    }

    public async Task RevokeAsync(Guid userId, string sessionId, CancellationToken ct)
    {
        await uow.GetEntityRepository<UserSession>().DbSet
            .Where(s => s.UserId == userId && s.SessionId == sessionId && s.RevokedAtUtc == null)
            .ExecuteUpdateAsync(u => u.SetProperty(s => s.RevokedAtUtc, _ => DateTime.UtcNow), ct);
    }
}
