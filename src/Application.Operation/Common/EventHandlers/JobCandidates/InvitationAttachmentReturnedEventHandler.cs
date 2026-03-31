using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Entities.Users;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Utils;
using Tawtheef.Domain.Events.Operation;

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
                "InvitationAttachmentReturned",
                user.Id,
                user.Email,
                "Action Required: Job Application Attachment Returned",
                null, null,
                payload);
            await unitOfWork.GetEntityRepository<Notification>().AddAsync(emailNotif, cancellationToken);
        }

        // In-App
        var inAppNotif = Notification.Create(
                NotificationChannel.InApp,
                "InvitationAttachmentReturned",
                user.Id,
                user.Email,
                "Action Required: Job Application Attachment Returned",
                null, null,
                payload);
            await unitOfWork.GetEntityRepository<Notification>().AddAsync(inAppNotif, cancellationToken);


        // SMS
        if (!string.IsNullOrEmpty(user.PhoneNumber) && MoiUtils.IsQatarMobileNumber(user.PhoneNumber!))
        {
            var smsNotif = Notification.Create(
                NotificationChannel.Sms,
                "InvitationAttachmentReturned",
                user.Id,
                user.PhoneNumber,
                "Attachment Returned",
                null, null,
                payload);
            await unitOfWork.GetEntityRepository<Notification>().AddAsync(smsNotif, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
