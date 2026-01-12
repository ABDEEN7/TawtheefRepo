using Microsoft.AspNetCore.Identity;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Common.Events.Job;

internal static class JobNotificationEmailHelper
{
    internal static async Task SendToHrAdminsAsync(
        UserManager<User> userManager,
        string subject,
        string body,
        CancellationToken ct)
    {
        var users = await userManager.GetUsersInRoleAsync(nameof(SystemRoleIds.Employee));
        if (users.Count == 0)
            return;

        foreach (var user in users)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                continue;
            }

            //TODO: should be used Event
            await emailSender.SendAsync(user.Email, subject, body, ct);
        }
    }
}
