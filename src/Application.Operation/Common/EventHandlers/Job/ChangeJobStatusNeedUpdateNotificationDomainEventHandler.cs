using System.Text.Json;
using Cortex.Mediator.Notifications;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
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
        var jobTitle = localizationService.GetLocalizedValue(notification.Job.TitleAr, notification.Job.TitleEn);
        var payload = JsonSerializer.Serialize(new ChangeJobStatusNeedUpdateNotificationModel(jobTitle));
        await JobNotificationEmailHelper.QueueForEmployeeAsync(
            unitOfWork,
            userManager,
            notification.Job.CreatedById.ToString()!,
            ChangeJobStatusNeedUpdateNotification.TemplateKey,
            "Job Update Required",
            payload,
            ct);
    }
}
