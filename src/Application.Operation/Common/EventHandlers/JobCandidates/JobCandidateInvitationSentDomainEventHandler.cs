using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.JobCandidates;
using Tawtheef.Notifications.Templates.JobCandidateInvitationSent;

namespace Application.Operation.Common.EventHandlers.JobCandidates;

public sealed class JobCandidateInvitationSentDomainEventHandler(IUnitOfWork unitOfWork, UserManager<User> userManager)
    : INotificationHandler<JobCandidateInvitationSentDomainEvent>
{
    public async Task Handle(JobCandidateInvitationSentDomainEvent request, CancellationToken ct)
    {
        var user = await userManager.Users.OfType<ApplicantUser>()
            .FirstOrDefaultAsync(u => u.Id == request.ApplicantId, ct);
        var lang = user?.PreferredLanguage ?? "ar";

        var notificationRepository = unitOfWork.GetEntityRepository<Notification>();
        var jobTitle = request.JobTitle;


        // Email Notification
        var payload = JsonSerializer.Serialize(new JobCandidateInvitationSentModel(jobTitle));

        // In-App Notification
        var inAppNotification = Notification.Create(
            NotificationChannel.InApp,
            JobCandidateInvitationSent.TemplateKey,
            request.ApplicantId,
            request.Email,
            null,
            null,
            null,
            payload,
            lang, null, 3);

        await notificationRepository.AddAsync(inAppNotification, ct);
        
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailNotification = Notification.Create(
                NotificationChannel.Email,
                JobCandidateInvitationSent.TemplateKey,
                request.ApplicantId,
                request.Email,
                null,
                null,
                null,
                payload, lang, null, 3);

            await notificationRepository.AddAsync(emailNotification, ct);
        }

        // SMS Notification
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {

            var smsNotification = Notification.Create(
                NotificationChannel.Sms,
                JobCandidateInvitationSent.TemplateKey,
                request.ApplicantId,
                request.PhoneNumber,
                null,
                null,
                null,
                payload, lang, null, 3);

            await notificationRepository.AddAsync(smsNotification, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}

