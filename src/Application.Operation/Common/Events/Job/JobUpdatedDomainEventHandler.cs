using System.Text.Json;
using Application.Operation.Templates.JobUpdatedNotification;
using Cortex.Mediator.Notifications;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;

namespace Application.Operation.Common.Events.Job;

public sealed class JobUpdatedDomainEventHandler(
    ILocalizationService localizationService,
    UserManager<User> userManager,
    IUnitOfWork unitOfWork)
    : INotificationHandler<JobUpdatedDomainEvent>
{
    public async Task Handle(JobUpdatedDomainEvent notification, CancellationToken ct)
    {
        var jobTitle = localizationService.GetLocalizedValue(notification.Job.TitleAr, notification.Job.TitleEn);
        var payload = JsonSerializer.Serialize(new JobUpdatedNotificationModel(jobTitle));
        await JobNotificationEmailHelper.QueueForHrAdminsAsync(
            unitOfWork,
            userManager,
            nameof(JobUpdatedNotification),
            "Job Posting Updated",
            payload,
            ct);
    }
}
