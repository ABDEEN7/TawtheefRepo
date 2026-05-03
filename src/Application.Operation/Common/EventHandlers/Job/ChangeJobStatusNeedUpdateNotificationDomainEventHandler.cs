using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;
using Tawtheef.Notifications.Templates.ChangeJobStatusNeedUpdateNotification;

namespace Application.Operation.Common.EventHandlers.Job;

public sealed class ChangeJobStatusNeedUpdateNotificationDomainEventHandler(
    UserManager<User> userManager,
    ILocalizationService localizationService,
    IUnitOfWork unitOfWork)
    : INotificationHandler<ChangeJobStatusNeedUpdateNotificationDomainEvent>
{
    public async Task Handle(ChangeJobStatusNeedUpdateNotificationDomainEvent notification, CancellationToken ct)
    {
        var jobTitle = localizationService.GetLocalizedValue(
            notification.Job.JobTitle?.JobNameAr ?? string.Empty,
            notification.Job.JobTitle?.JobNameEn ?? string.Empty);
        var payload = JsonSerializer.Serialize(new ChangeJobStatusNeedUpdateNotificationModel(jobTitle));

        var userId = notification.Job.CreatedById.ToString()!;
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return;

        var lang = user.GetPreferredLanguage();
        var repo = unitOfWork.GetEntityRepository<Notification>();

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            // Email
            var emailNotification = Notification.Create(NotificationChannel.Email,
                ChangeJobStatusNeedUpdateNotification.TemplateKey, user.Id,
                user.Email, null, null, null, 
                payload, lang);
            await repo.AddAsync(emailNotification, ct);
        }

        // In-App
        var inAppNotification = Notification.Create(NotificationChannel.InApp, 
            ChangeJobStatusNeedUpdateNotification.TemplateKey, user.Id, 
            user.Email, null, null, null, 
            payload, lang);
        await repo.AddAsync(inAppNotification, ct);

        await unitOfWork.SaveChangesAsync(ct);
    }
}

