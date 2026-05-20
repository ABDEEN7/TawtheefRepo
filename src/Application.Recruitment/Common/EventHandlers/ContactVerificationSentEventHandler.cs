using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.User;
using Tawtheef.Notifications.Templates.ContactVerificationSent;

namespace Application.Recruitment.Common.EventHandlers;

public sealed class ContactVerificationSentEventHandler(IUnitOfWork uow, UserManager<User> userManager)
    : INotificationHandler<ContactVerificationSentEvent>
{
    public async Task Handle(ContactVerificationSentEvent @event, CancellationToken cancellationToken)
    {
        var channel = @event.Type == ContactVerificationType.Email ? NotificationChannel.Email : NotificationChannel.Sms;
        var payload = JsonSerializer.Serialize(new ContactVerificationSentModel(@event.Code));
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == @event.UserId, cancellationToken);
        var preferredLanguage= user?.PreferredLanguage ?? "ar";
        var notification = Notification.Create(
            channel,
            ContactVerificationSent.TemplateKey,
            @event.UserId,
            @event.Destination,
            null,
            null,
            null,
            payload, preferredLanguage);
        await uow.GetEntityRepository<Notification>().AddAsync(notification, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
    }
}

