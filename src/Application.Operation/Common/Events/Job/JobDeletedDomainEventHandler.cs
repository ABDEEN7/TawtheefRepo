using Cortex.Mediator.Notifications;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;

namespace Application.Operation.Common.Events.Job;

public sealed class JobDeletedDomainEventHandler(
    UserManager<User> userManager)
    : INotificationHandler<JobDeletedDomainEvent>
{
    public async Task Handle(JobDeletedDomainEvent notification, CancellationToken ct)
    {
        //TODO: should be add Localization for Title property
        var jobTitle = notification.Job.TitleEn;
        var body = $"""
            The job ({jobTitle}) has been deleted.

            Please log in to the Tawtheef system to review the job changes.
            """;

        //TODO: should be add to notification table
        await JobNotificationEmailHelper.SendToHrAdminsAsync(
            emailSender,
            userManager,
            "Job Deleted",
            body,
            ct);
    }
}
