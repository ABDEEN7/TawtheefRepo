using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Common.EventHandlers.Job;

internal static class JobNotificationEmailHelper
{
    internal static async Task QueueForDepartmentManagerAsync(IUnitOfWork unitOfWork, UserManager<User> userManager, 
        string templateKey, string payloadJson, CancellationToken ct)
    {
        var users = await userManager.GetUsersInRoleAsync(nameof(SystemRoleIds.DepartmentManager));
        if (users.Count == 0)
            return;

        var repo = unitOfWork.GetEntityRepository<Notification>();

        foreach (var user in users)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
                continue;

            var lang = user.GetPreferredLanguage();
            
            // Email
            var emailNotification = Notification.Create(NotificationChannel.Email, templateKey, user.Id, 
                user.Email, null, null, null, payloadJson, lang, null, 3);
            await repo.AddAsync(emailNotification, ct);

            // In-App
            var inAppNotification = Notification.Create(NotificationChannel.InApp, templateKey, user.Id, 
                user.Email, null, null, null, payloadJson, lang, null, 3);
            await repo.AddAsync(inAppNotification, ct);
        }
        await unitOfWork.SaveChangesAsync(ct);
    }
    
    internal static async Task QueueForEmployeeAsync(IUnitOfWork unitOfWork, UserManager<User> userManager, 
        string userId, string templateKey, string payloadJson, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user == null || string.IsNullOrWhiteSpace(user.Email))
            return;

        var lang = user.GetPreferredLanguage();
        var repo = unitOfWork.GetEntityRepository<Notification>();
        
        // Email
        var emailNotification = Notification.Create(NotificationChannel.Email, templateKey, user.Id, 
            user.Email, null, null, null, payloadJson, lang, null, 3);
        await repo.AddAsync(emailNotification, ct);

        // In-App
        var inAppNotification = Notification.Create(NotificationChannel.InApp, templateKey, user.Id, 
            user.Email, null, null, null, payloadJson, lang, null, 3);
        await repo.AddAsync(inAppNotification, ct);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
