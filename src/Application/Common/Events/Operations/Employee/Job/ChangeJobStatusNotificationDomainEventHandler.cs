using Cortex.Mediator.Notifications;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;

namespace Tawtheef.Application.Common.Events.Operations.Employee.Job;

public sealed class NewJobRequiresReviewDomainEventHandler(
    IEmailSender emailSender,
    UserManager<User> userManager)
    : INotificationHandler<ChangeJobStatusNotificationDomainEvent>
{
    public async Task Handle(
        ChangeJobStatusNotificationDomainEvent notification,
        CancellationToken ct)
    {
        if (notification is null)
            return;

        var users = await userManager.GetUsersInRoleAsync("HRAdmin");

        if (users is null || users.Count == 0)
            return;

        foreach (var user in users)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
                continue;

            var recipientName = !string.IsNullOrWhiteSpace(user.FullNameAr)
                ? user.FullNameAr
                : user.UserName;

            var body = $"""
                Dear {recipientName},

                A new job has been created and requires your review.

                Please log in to the Tawtheef system to review the job details
                and take the necessary action.

                Best regards,
                Tawtheef System
                """;

            await emailSender.SendAsync(
                user.Email,
                "New Job Requires Review",
                body,
                ct);
        }
    }
}
