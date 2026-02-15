using System.Text.Json;
using Cortex.Mediator.Notifications;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;
using Tawtheef.Notifications.Templates.ChangeJobStatusRejectedNotification;

namespace Application.Operation.Common.EventHandlers.Job;

public sealed class ChangeJobStatusRejectedNotificationDomainEventHandler(
    UserManager<User> userManager,
    ILocalizationService localizationService,
    IUnitOfWork unitOfWork)
    : INotificationHandler<ChangeJobStatusRejectedNotificationDomainEvent>
{
    public async Task Handle(ChangeJobStatusRejectedNotificationDomainEvent notification, CancellationToken ct)
    {
        var jobTitle = localizationService.GetLocalizedValue(notification.Job.JobTitle?.JobNameAr ?? string.Empty, notification.Job.JobTitle?.JobNameEn ?? string.Empty);
        var payload = JsonSerializer.Serialize(new ChangeJobStatusRejectedNotificationModel(jobTitle));
        await JobNotificationEmailHelper.QueueForEmployeeAsync(
            unitOfWork,
            userManager,
            notification.Job.CreatedById.ToString()!,
            ChangeJobStatusRejectedNotification.TemplateKey,
            "Job Request Rejected",
            payload,
            ct);
    }
}
