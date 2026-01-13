using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Common.Events.Job;

internal static class JobNotificationEmailHelper
{
    internal static async Task QueueForHrAdminsAsync(
        IUnitOfWork unitOfWork,
        UserManager<User> userManager,
        string templateKey,
        string subject,
        string payloadJson,
        CancellationToken ct)
    {
        var users = await userManager.GetUsersInRoleAsync(nameof(SystemRoleIds.Employee));
        if (users.Count == 0)
            return;

        var repo = unitOfWork.GetEntityRepository<Notification>();

        foreach (var user in users)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
                continue;

            var notification = Notification.Create(
                NotificationChannel.Email,
                templateKey,
                user.Id,
                user.Email,
                subject,
                null,
                payloadJson);

            await repo.AddAsync(notification);
        }
    }
}
