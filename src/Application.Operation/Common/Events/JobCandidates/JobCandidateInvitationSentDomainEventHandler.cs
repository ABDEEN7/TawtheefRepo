using System.Text.Json;
using Application.Operation.Templates.JobCandidateInvitationSent;
using Cortex.Mediator.Notifications;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.Operation.Employee.JobCandidates;

namespace Application.Operation.Common.Events.JobCandidates;

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
            var emailNotification = Notification.Create(
                NotificationChannel.Email,
                nameof(JobCandidateInvitationSent),
                notification.ApplicantId,
                notification.Email,
                "Job Invitation",
                null,
                payload);
            await repo.AddAsync(emailNotification);
        }

        if (!string.IsNullOrWhiteSpace(notification.PhoneNumber))
        {
            var body = string.IsNullOrWhiteSpace(jobTitle)
                ? "You have been invited to apply for a job on Tawtheef."
                : $"You have been invited to apply for {jobTitle} on Tawtheef.";

            var smsNotification = Notification.Create(
                NotificationChannel.Sms,
                nameof(JobCandidateInvitationSent),
                notification.ApplicantId,
                notification.PhoneNumber,
                "Job Invitation",
                body,
                null);
            await repo.AddAsync(smsNotification);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}
