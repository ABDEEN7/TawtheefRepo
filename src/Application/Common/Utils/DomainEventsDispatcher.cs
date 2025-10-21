using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Application.Common.Utils;

public static class DomainEventsDispatcher
{
    public static async Task DispatchAsync(IMediator mediator, DbContext ctx, CancellationToken ct = default)
    {
        var entities = ctx.ChangeTracker.Entries()
            .Select(e => e.Entity)
            .OfType<IHasDomainEvents>()
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var events = entities.SelectMany(e => e.DomainEvents).ToList();
        foreach (var domainEvent in events)
            await mediator.Publish(domainEvent, ct);

        entities.ForEach(e => e.ClearDomainEvents());
    }
}