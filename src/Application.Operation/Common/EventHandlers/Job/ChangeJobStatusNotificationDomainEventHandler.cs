using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;
using Tawtheef.Notifications.Templates.ChangeJobStatusNotification;

namespace Application.Operation.Common.EventHandlers.Job;

public sealed class ChangeJobStatusNotificationDomainEventHandler(
    UserManager<User> userManager,
    ILocalizationService localizationService,
    IUnitOfWork unitOfWork)
    : INotificationHandler<ChangeJobStatusNotificationDomainEvent>
{
    public async Task Handle(ChangeJobStatusNotificationDomainEvent notification, CancellationToken ct)
    {
        var jobTitle = localizationService.GetLocalizedValue(notification.Job.JobTitle?.JobNameAr ?? string.Empty, notification.Job.JobTitle?.JobNameEn ?? string.Empty);
        var payload = JsonSerializer.Serialize(new ChangeJobStatusNotificationModel(jobTitle));
        await JobNotificationEmailHelper.QueueForDepartmentManagerAsync(
            unitOfWork,
            userManager,
            ChangeJobStatusNotification.TemplateKey,
            "Job Status Review Required",
            payload,
            ct);
    }
}

