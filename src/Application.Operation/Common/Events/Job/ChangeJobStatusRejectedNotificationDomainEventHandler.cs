using System.Text.Json;
using Application.Operation.Templates.ChangeJobStatusRejectedNotification;
using Cortex.Mediator.Notifications;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;

namespace Application.Operation.Common.Events.Job;

public sealed class ChangeJobStatusRejectedNotificationDomainEventHandler(
    UserManager<User> userManager,
    ILocalizationService localizationService,
    IUnitOfWork unitOfWork)
    : INotificationHandler<ChangeJobStatusRejectedNotificationDomainEvent>
{
    public async Task Handle(ChangeJobStatusRejectedNotificationDomainEvent notification, CancellationToken ct)
    {
        var jobTitle = localizationService.GetLocalizedValue(notification.Job.TitleAr, notification.Job.TitleEn);
        var payload = JsonSerializer.Serialize(new ChangeJobStatusRejectedNotificationModel(jobTitle));
        await JobNotificationEmailHelper.QueueForEmployeeAsync(
            unitOfWork,
            userManager,
            notification.Job.CreatedById.ToString()!,
            nameof(ChangeJobStatusRejectedNotification),
            "Job Request Rejected",
            payload,
            ct);
    }
}
