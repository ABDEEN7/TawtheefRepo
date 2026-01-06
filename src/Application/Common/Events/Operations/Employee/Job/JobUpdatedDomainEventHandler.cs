using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;

namespace Tawtheef.Application.Common.Events.Operations.Employee.Job;

public sealed class JobUpdatedDomainEventHandler(
    IEmailSender emailSender,
    UserManager<User> userManager)
    : INotificationHandler<JobUpdatedDomainEvent>
{
    public async Task Handle(JobUpdatedDomainEvent notification, CancellationToken ct)
    {
        if (notification is null)
        {
            return;
        }

        var jobTitle = notification.Job.TitleEn ?? notification.Job.TitleAr ?? "a job";
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
