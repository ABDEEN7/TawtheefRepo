using Cortex.Mediator.Notifications;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;

namespace Application.Operation.Common.Events.Job;

public sealed class JobUpdatedDomainEventHandler(
    IEmailSender emailSender,
    ILocalizationService localizationService,
    UserManager<User> userManager)
    : INotificationHandler<JobUpdatedDomainEvent>
{
    public async Task Handle(JobUpdatedDomainEvent notification, CancellationToken ct)
    {
        var jobTitle = localizationService.GetLocalizedValue(notification.Job.TitleAr, notification.Job.TitleEn);
        var body = $"""
            The job ({jobTitle}) has been updated.

            Please log in to the Tawtheef system to review the updated details.
            """;

        await JobNotificationEmailHelper.SendToHrAdminsAsync(
            emailSender,
            userManager,
            "Job Updated",
            body,
            ct);
    }
}
