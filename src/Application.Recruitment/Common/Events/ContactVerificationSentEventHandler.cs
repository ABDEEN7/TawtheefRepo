using System.Text.Json;
using Application.Recruitment.Templates.ContactVerificationSent;
using Cortex.Mediator.Notifications;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.User;

namespace Application.Recruitment.Common.Events;

public sealed class ContactVerificationSentEventHandler(IUnitOfWork uow)
    : INotificationHandler<ContactVerificationSentEvent>
{
    public async Task Handle(ContactVerificationSentEvent @event, CancellationToken cancellationToken)
    {
        var channel = @event.Type == ContactVerificationType.Email ? NotificationChannel.Email : NotificationChannel.Sms;
        var payload = JsonSerializer.Serialize(new ContactVerificationSentModel(@event.Code));
        var notification = Notification.Create(
            channel,
            nameof(ContactVerificationSent),
            @event.UserId,
            @event.Destination,
            "Tawtheef Verification Code",
            null,
            payload);
        await uow.GetEntityRepository<Notification>().AddAsync(notification);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
