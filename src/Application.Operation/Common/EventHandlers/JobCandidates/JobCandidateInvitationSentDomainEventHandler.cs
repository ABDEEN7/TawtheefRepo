using System.Text.Json;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.Operation.Employee.JobCandidates;
using Tawtheef.Notifications.Templates.JobCandidateInvitationSent;

namespace Application.Operation.Common.EventHandlers.JobCandidates;

public sealed class JobCandidateInvitationSentDomainEventHandler(IUnitOfWork unitOfWork)
    : INotificationHandler<JobCandidateInvitationSentDomainEvent>
{public async Task Handle(JobCandidateInvitationSentDomainEvent request, CancellationToken ct)
    {
        var notificationRepository = unitOfWork.GetEntityRepository<Notification>();
        var jobTitle = request.JobTitle;

        const string subject = "Careers Job Invitation";

        // Email Notification
        var payload = JsonSerializer.Serialize(new JobCandidateInvitationSentModel(jobTitle));

        // In-App Notification
        var inAppNotification = Notification.Create(
            NotificationChannel.InApp,
            JobCandidateInvitationSent.TemplateKey,
            request.ApplicantId,
            request.Email,
            subject,
            null,
            payload);

        await notificationRepository.AddAsync(inAppNotification, ct);
        
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailNotification = Notification.Create(
                NotificationChannel.Email,
                JobCandidateInvitationSent.TemplateKey,
                request.ApplicantId,
                request.Email,
                subject,
                null,
                payload);

            await notificationRepository.AddAsync(emailNotification, ct);
        }

        // SMS Notification
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var messageBody = string.IsNullOrWhiteSpace(jobTitle)
                ? JobCandidatesMessages.JobInvitationWithoutTitle
                : string.Format(JobCandidatesMessages.JobInvitationWithTitle, jobTitle);

            var smsNotification = Notification.Create(
                NotificationChannel.Sms,
                JobCandidateInvitationSent.TemplateKey,
                request.ApplicantId,
                request.PhoneNumber,
                subject,
                messageBody,
                null);

            await notificationRepository.AddAsync(smsNotification, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}

