using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;
using Tawtheef.Notifications.Templates.JobUpdatedNotification;

namespace Application.Operation.Common.EventHandlers.Job;

public sealed class JobUpdatedDomainEventHandler(
    ILocalizationService localizationService,
    UserManager<User> userManager,
    IUnitOfWork unitOfWork)
    : INotificationHandler<JobUpdatedDomainEvent>
{
    public async Task Handle(JobUpdatedDomainEvent notification, CancellationToken ct)
    {
        var jobTitle = localizationService.GetLocalizedValue(notification.Job.JobTitle?.JobNameAr ?? string.Empty, notification.Job.JobTitle?.JobNameEn ?? string.Empty);
        var payload = JsonSerializer.Serialize(new JobUpdatedNotificationModel(jobTitle));
        await JobNotificationEmailHelper.QueueForDepartmentManagerAsync(
            unitOfWork,
            userManager,
            JobUpdatedNotification.TemplateKey,
            payload,
            ct);
    }
}

