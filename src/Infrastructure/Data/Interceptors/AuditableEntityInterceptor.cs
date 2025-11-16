using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Common;

namespace Tawtheef.Infrastructure.Data.Interceptors;

public class AuditableEntityInterceptor(
    ICurrentUserService user,
    TimeProvider dateTime) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static Guid? ParseUserIdOrNull(string? id)
        => Guid.TryParse(id, out var g) && g != Guid.Empty ? g : null;

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var currentUserId = ParseUserIdOrNull(user.UserId);
        var utcNow = dateTime.GetUtcNow();

        foreach (var entry in context.ChangeTracker.Entries<EventEntity>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedById = currentUserId;
                    entry.Entity.CreatedDate = utcNow;
                }

                entry.Entity.UpdatedById = currentUserId;
                entry.Entity.UpdatedDate = utcNow;
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
