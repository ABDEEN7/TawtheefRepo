using Cortex.Mediator.Notifications;
using Tawtheef.Domain.Events.Operation.Employee.Office;

namespace Application.Operation.Common.Events.JobCandidates;

public sealed class OfficeCreatedDomainEventHandler
    : INotificationHandler<OfficeCreatedDomainEvent>
{
    public async Task Handle(OfficeCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        //TODO: Send notification to the office admin
        await Task.Delay(1000, cancellationToken);
    }
}
