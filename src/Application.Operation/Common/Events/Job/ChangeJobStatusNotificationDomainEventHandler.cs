using Cortex.Mediator.Notifications;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;

namespace Application.Operation.Common.Events.Job;

public sealed class NewJobRequiresReviewDomainEventHandler(UserManager<User> userManager)
    : INotificationHandler<ChangeJobStatusNotificationDomainEvent>
{
    public async Task Handle(ChangeJobStatusNotificationDomainEvent notification,CancellationToken ct)
    {
        var users = await userManager.GetUsersInRoleAsync(nameof(SystemRoleIds.Employee));
        if (users.Count == 0)
            return;

        foreach (var user in users.Where(u =>  !string.IsNullOrEmpty(u.Email)))
        {
            //TODO: should be used Event
            emailSender.SendAsync(user.Email, "New Job Requires Review", "New Job Requires Review", ct);
        }
    }
}
