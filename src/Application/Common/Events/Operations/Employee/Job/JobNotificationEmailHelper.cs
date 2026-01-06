using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Events.Operations.Employee.Job;

internal static class JobNotificationEmailHelper
{
    internal static async Task SendToHrAdminsAsync(
        IEmailSender emailSender,
        UserManager<User> userManager,
        string subject,
        string body,
        CancellationToken ct)
    {
        var users = await userManager.GetUsersInRoleAsync("HRAdmin");
        if (users is null || users.Count == 0)
        {
            return;
        }

        foreach (var user in users)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                continue;
            }

            await emailSender.SendAsync(user.Email, subject, body, ct);
        }
    }
}
