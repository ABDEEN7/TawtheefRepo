using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;
using Tawtheef.Notifications.Templates.ChangeJobStatusNotification;

namespace Application.Operation.Common.EventHandlers.Job;

public sealed class ChangeJobStatusNotificationDomainEventHandler(
    UserManager<User> userManager,
    ILocalizationService localizationService,
    IUnitOfWork unitOfWork)
    : INotificationHandler<ChangeJobStatusNotificationDomainEvent>
{
    public async Task Handle(ChangeJobStatusNotificationDomainEvent notification, CancellationToken ct)
    {
        var jobTitle = localizationService.GetLocalizedValue(notification.Job.JobTitle?.JobNameAr ?? string.Empty, notification.Job.JobTitle?.JobNameEn ?? string.Empty);
        var payload = JsonSerializer.Serialize(new ChangeJobStatusNotificationModel(jobTitle));
        
        var users = await userManager.GetUsersInRoleAsync(nameof(SystemRoleIds.DepartmentManager));
        if (users.Count == 0)
            return;

        var repo = unitOfWork.GetEntityRepository<Notification>();

        foreach (var user in users)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
                continue;

            var lang = user.GetPreferredLanguage();
            
            // Email
            var emailNotification = Notification.Create(NotificationChannel.Email, 
                ChangeJobStatusNotification.TemplateKey, user.Id, 
                user.Email, null, null, null, payload, lang);
            await repo.AddAsync(emailNotification, ct);

            // In-App
            var inAppNotification = Notification.Create(NotificationChannel.InApp, 
                ChangeJobStatusNotification.TemplateKey, user.Id, 
                user.Email, null, null, null, payload, lang);
            await repo.AddAsync(inAppNotification, ct);
        }
        await unitOfWork.SaveChangesAsync(ct);
    }
}

