using System.Text.Json;
using Cortex.Mediator.Notifications;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;
using Tawtheef.Notifications.Templates.JobCreatedNotification;

namespace Application.Operation.Common.EventHandlers.Job;

public sealed class JobCreatedDomainEventHandler(
    UserManager<User> userManager,
    ILocalizationService localizationService,
    IUnitOfWork unitOfWork)
    : INotificationHandler<JobCreatedDomainEvent>
{
    public async Task Handle(JobCreatedDomainEvent notification, CancellationToken ct)
    {
        var jobTitle = localizationService.GetLocalizedValue(notification.Job.JobTitle?.JobNameAr ?? string.Empty, notification.Job.JobTitle?.JobNameEn ?? string.Empty);
        var payload = JsonSerializer.Serialize(new JobCreatedNotificationModel(jobTitle));
        await JobNotificationEmailHelper.QueueForDepartmentManagerAsync(
            unitOfWork,
            userManager,
            JobCreatedNotification.TemplateKey,
            "New Job Posting Created",
            payload,
            ct);
    }
}
