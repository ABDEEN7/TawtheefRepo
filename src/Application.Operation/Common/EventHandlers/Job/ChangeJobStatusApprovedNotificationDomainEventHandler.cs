using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;
using Tawtheef.Notifications.Templates.ChangeJobStatusApprovedNotification;

namespace Application.Operation.Common.EventHandlers.Job;

public sealed class ChangeJobStatusApprovedNotificationDomainEventHandler(
    UserManager<User> userManager,
    ILocalizationService localizationService,
    IUnitOfWork unitOfWork)
    : INotificationHandler<ChangeJobStatusApprovedNotificationDomainEvent>
{
    public async Task Handle(ChangeJobStatusApprovedNotificationDomainEvent eventData, CancellationToken ct)
    {
        var jobTitle = localizationService.GetLocalizedValue(eventData.Job.JobTitle?.JobNameAr ?? string.Empty, eventData.Job.JobTitle?.JobNameEn ?? string.Empty);
        var payload = JsonSerializer.Serialize(new ChangeJobStatusApprovedNotificationModel(jobTitle));
        
        var userId = eventData.Job.CreatedById.ToString()!;
        var user = await userManager.FindByIdAsync(userId);
        if (user == null || string.IsNullOrWhiteSpace(user.Email))
            return;

        var lang = user.GetPreferredLanguage();
        var repo = unitOfWork.GetEntityRepository<Notification>();
        
        // Email
        var emailNotification = Notification.Create(NotificationChannel.Email, 
            ChangeJobStatusApprovedNotification.TemplateKey, user.Id, 
            user.Email, null, null, null, payload, lang);
        await repo.AddAsync(emailNotification, ct);

        // In-App
        var inAppNotification = Notification.Create(NotificationChannel.InApp, 
            ChangeJobStatusApprovedNotification.TemplateKey, user.Id, 
            user.Email, null, null, null, payload, lang);
        await repo.AddAsync(inAppNotification, ct);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
