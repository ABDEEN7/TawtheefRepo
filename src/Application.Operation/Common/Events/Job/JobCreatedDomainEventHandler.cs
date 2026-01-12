using Cortex.Mediator.Notifications;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;

namespace Application.Operation.Common.Events.Job;

public sealed class JobCreatedDomainEventHandler(
    UserManager<User> userManager)
    : INotificationHandler<JobCreatedDomainEvent>
{
    public async Task Handle(JobCreatedDomainEvent notification, CancellationToken ct)
    {
        //TODO: should be add Localization for Title property
        var jobTitle = notification.Job.TitleEn;
        var body = $"""
            A new job ({jobTitle}) has been created and is ready for review.

            Please log in to the Tawtheef system to review the job details.
            """;

        //TODO: should be add to notification table
        await JobNotificationEmailHelper.SendToHrAdminsAsync(
            emailSender,
            userManager,
            "New Job Created",
            body,
            ct);
    }
}
