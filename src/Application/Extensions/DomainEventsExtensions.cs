using MediatR;
using Tawtheef.Domain.Common;

namespace Tawtheef.Application.Extensions;

public static class DomainEventsExtensions
{
    public static async Task PublishAndClearAsync(this IHasDomainEvents entity, IMediator mediator, CancellationToken ct)
    {
        foreach (var e in entity.DomainEvents) 
            await mediator.Publish(e, ct);
        entity.ClearDomainEvents();
    }
}
