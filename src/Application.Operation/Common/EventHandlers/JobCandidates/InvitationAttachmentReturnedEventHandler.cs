using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Utils;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation;
using Tawtheef.Notifications.Templates.InvitationAttachmentReturned;

namespace Application.Operation.Common.EventHandlers.JobCandidates;

public class InvitationAttachmentReturnedEventHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IAppLogger logger)
    : INotificationHandler<InvitationAttachmentReturnedEvent>
{
    public async Task Handle(InvitationAttachmentReturnedEvent notification, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .OfType<ApplicantUser>()
            .FirstOrDefaultAsync(u => u.Id == notification.ApplicantId, cancellationToken);
        if (user is null)
        {
            logger.Error("User with id {UserId} not found for InvitationAttachmentReturnedEvent", notification.ApplicantId);
            return;
        }

        var payload = JsonSerializer.Serialize(new { 
            AttachmentTitle = notification.AttachmentTitle, 
            ReviewNote = notification.ReviewNote 
        });

        // Email
        if (!string.IsNullOrEmpty(user.Email))
        {
            var emailNotif = Notification.Create(
                NotificationChannel.Email,
                InvitationAttachmentReturned.TemplateKey,
                user.Id,
                user.Email,
                null,
                null,
                null,
                payload, user.GetPreferredLanguage(), null, 3);
            await unitOfWork.GetEntityRepository<Notification>().AddAsync(emailNotif, cancellationToken);
        }

        // In-App
        var inAppNotif = Notification.Create(
                NotificationChannel.InApp,
                InvitationAttachmentReturned.TemplateKey,
                user.Id,
                user.Email,
                null,
                null,
                null,
                payload, user.GetPreferredLanguage(), null, 3);
            await unitOfWork.GetEntityRepository<Notification>().AddAsync(inAppNotif, cancellationToken);


        // SMS
        if (!string.IsNullOrEmpty(user.PhoneNumber) && MoiUtils.IsQatarMobileNumber(user.PhoneNumber!))
        {
            var smsNotif = Notification.Create(
                NotificationChannel.Sms,
                InvitationAttachmentReturned.TemplateKey,
                user.Id,
                user.PhoneNumber,
                null,
                null,
                null,
                payload, user.GetPreferredLanguage(), null, 3);
            await unitOfWork.GetEntityRepository<Notification>().AddAsync(smsNotif, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
