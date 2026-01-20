using Cortex.Mediator;
using Microsoft.EntityFrameworkCore;

namespace Tawtheef.Domain.Common;


public class DomainEventDispatcher
{
    public static async Task DispatchAsync(IMediator mediator, DbContext ctx, CancellationToken ct = default)
    {
        var entities = ctx.ChangeTracker
            .Entries()
            .Select(e => e.Entity)
            .OfType<IHasDomainEvents>()         // <- interface filter
            .Where(e => e.DomainEvents.Count != 0)
            .ToList();

        var events = entities.SelectMany(e => e.DomainEvents).ToList();
        foreach (var domainEvent in events)
            //TODO: Cast to dynamic to avoid compile-time error.
            await mediator.PublishAsync((dynamic)domainEvent, ct);

        entities.ForEach(e => e.ClearDomainEvents());
    }
}
