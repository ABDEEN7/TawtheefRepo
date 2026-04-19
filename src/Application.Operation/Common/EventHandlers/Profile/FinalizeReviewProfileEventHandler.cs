using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Profile;
using Tawtheef.Notifications.Templates.FinalizeReviewProfile;

namespace Application.Operation.Common.EventHandlers.Profile;

public class FinalizeReviewProfileEventHandler(IUnitOfWork uow, UserManager<User> userManager, IAppLogger logger)
    : INotificationHandler<FinalizeReviewProfileEvent>
{
    readonly IAppLogger _log = logger.ForContext(nameof(FinalizeReviewProfileEventHandler));
    public async Task Handle(FinalizeReviewProfileEvent @event, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(@event.UserId.ToString());
        if(user == null) {
            _log.Error("User with id {UserId} not found for FinalizeReviewProfileEvent", @event.UserId);
            return;
        }
        var payload = JsonSerializer.Serialize(new FinalizeReviewProfileModel(@event.Status));
        var lang = user.PreferredLanguage;
        var notificationEmail = Notification.Create(
            NotificationChannel.Email,
            FinalizeReviewProfile.TemplateKey,
            @event.UserId,
            user.Email,
            null,
            null,null, 
            payload,
            null, 3, lang);        
        var notificationInApp = Notification.Create(
            NotificationChannel.InApp,
            FinalizeReviewProfile.TemplateKey,
            @event.UserId,
            user.Email,
            null,
            null,null, 
            payload,
            null, 3, lang);
        await uow.GetEntityRepository<Notification>().AddAsync(notificationEmail, cancellationToken);
        await uow.GetEntityRepository<Notification>().AddAsync(notificationInApp, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
    }
}

