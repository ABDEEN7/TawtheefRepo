using System.Text.Json;
using Application.Operation.Templates.JobCreatedNotification;
using Cortex.Mediator.Notifications;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;

namespace Application.Operation.Common.Events.Job;

public sealed class JobCreatedDomainEventHandler(
    UserManager<User> userManager,
    ILocalizationService localizationService,
    IUnitOfWork unitOfWork)
    : INotificationHandler<JobCreatedDomainEvent>
{
    public async Task Handle(JobCreatedDomainEvent notification, CancellationToken ct)
    {
        var jobTitle = localizationService.GetLocalizedValue(notification.Job.TitleAr, notification.Job.TitleEn);
        var payload = JsonSerializer.Serialize(new JobCreatedNotificationModel(jobTitle));
        await JobNotificationEmailHelper.QueueForDepartmentManagerAsync(
            unitOfWork,
            userManager,
            nameof(JobCreatedNotification),
            "New Job Posting Created",
            payload,
            ct);
    }
}
