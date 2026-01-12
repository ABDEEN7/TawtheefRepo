using System.Text.Json;
using Application.Operation.Templates.JobDeletedNotification;
using Cortex.Mediator.Notifications;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;

namespace Application.Operation.Common.Events.Job;

public sealed class JobDeletedDomainEventHandler(
    UserManager<User> userManager,
    ILocalizationService localizationService,
    IUnitOfWork unitOfWork)
    : INotificationHandler<JobDeletedDomainEvent>
{
    public async Task Handle(JobDeletedDomainEvent notification, CancellationToken ct)
    {
        var jobTitle = localizationService.GetLocalizedValue(notification.Job.TitleAr, notification.Job.TitleEn);
        var payload = JsonSerializer.Serialize(new JobDeletedNotificationModel(jobTitle));
        await JobNotificationEmailHelper.QueueForHrAdminsAsync(
            unitOfWork,
            userManager,
            nameof(JobDeletedNotification),
            "Job Deleted",
            payload,
            ct);
    }
}
