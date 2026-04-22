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

        var user = await userManager.FindByIdAsync(eventData.Job.CreatedById.ToString()!);
        if (user == null || string.IsNullOrWhiteSpace(user.Email))
            return;

        var repo = unitOfWork.GetEntityRepository<Notification>();
        var notification = Notification.Create(NotificationChannel.InApp, ChangeJobStatusApprovedNotification.TemplateKey, user.Id, 
            user.Email, null, null, null, payload, null, 3, user.PreferredLanguage);
        await repo.AddAsync(notification, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}

