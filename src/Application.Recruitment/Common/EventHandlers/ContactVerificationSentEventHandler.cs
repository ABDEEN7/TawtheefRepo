using System.Text.Json;
using Cortex.Mediator.Notifications;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.User;
using Tawtheef.Notifications.Templates.ContactVerificationSent;

namespace Application.Recruitment.Common.EventHandlers;

public sealed class ContactVerificationSentEventHandler(IUnitOfWork uow)
    : INotificationHandler<ContactVerificationSentEvent>
{
    public async Task Handle(ContactVerificationSentEvent @event, CancellationToken cancellationToken)
    {
        var channel = @event.Type == ContactVerificationType.Email ? NotificationChannel.Email : NotificationChannel.Sms;
        var payload = JsonSerializer.Serialize(new ContactVerificationSentModel(@event.Code));
        var notification = Notification.Create(
            channel,
            ContactVerificationSent.TemplateKey,
            @event.UserId,
            @event.Destination,
            "Careers Verification Code",
            null,
            payload);
        await uow.GetEntityRepository<Notification>().AddAsync(notification, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
