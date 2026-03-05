using System.Text.Json;
using Cortex.Mediator.Notifications;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.Operation.Employee.JobCandidates;
using Tawtheef.Notifications.Templates.JobCandidateInvitationSent;

namespace Application.Operation.Common.EventHandlers.JobCandidates;

public sealed class JobCandidateInvitationSentDomainEventHandler(IUnitOfWork unitOfWork)
    : INotificationHandler<JobCandidateInvitationSentDomainEvent>
{
    public async Task Handle(JobCandidateInvitationSentDomainEvent notification, CancellationToken ct)
    {
        var repo = unitOfWork.GetEntityRepository<Notification>();
        var jobTitle = notification.JobTitle;

        if (!string.IsNullOrWhiteSpace(notification.Email))
        {
            var payload = JsonSerializer.Serialize(new JobCandidateInvitationSentModel(jobTitle));
            var emailNotification = Notification.Create( NotificationChannel.Email, 
                JobCandidateInvitationSent.TemplateKey, notification.ApplicantId, 
                notification.Email, "Careers Job Invitation", null, payload);
            await repo.AddAsync(emailNotification, ct);
        }

        if (!string.IsNullOrWhiteSpace(notification.PhoneNumber))
        {
            var body = string.IsNullOrWhiteSpace(jobTitle)
                ? JobCandidatesMessages.JobInvitationWithoutTitle
                : string.Format(JobCandidatesMessages.JobInvitationWithTitle, jobTitle);

            var smsNotification = Notification.Create( NotificationChannel.Sms, 
                nameof(JobCandidateInvitationSent), notification.ApplicantId, 
                notification.PhoneNumber, "Careers Job Invitation", body, null);
            await repo.AddAsync(smsNotification, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}
