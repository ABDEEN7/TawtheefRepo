using System.Text.Json;
using Tawtheef.Notifications.Templates.Operation.ChangeJobStatusApprovedNotification;
using Cortex.Mediator.Notifications;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;

namespace Application.Operation.Common.Events.Job;

public sealed class ChangeJobStatusApprovedNotificationDomainEventHandler(
    UserManager<User> userManager,
    ILocalizationService localizationService,
    IUnitOfWork unitOfWork)
    : INotificationHandler<ChangeJobStatusApprovedNotificationDomainEvent>
{
    public async Task Handle(ChangeJobStatusApprovedNotificationDomainEvent notification, CancellationToken ct)
    {
        var jobTitle = localizationService.GetLocalizedValue(notification.Job.TitleAr, notification.Job.TitleEn);
        var payload = JsonSerializer.Serialize(new ChangeJobStatusApprovedNotificationModel(jobTitle));
        await JobNotificationEmailHelper.QueueForEmployeeAsync(
            unitOfWork,
            userManager,
            notification.Job.CreatedById.ToString()!,
            nameof(ChangeJobStatusApprovedNotification),
            "Job Approval Confirmed",
            payload,
            ct);
    }
}
