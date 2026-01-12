using Cortex.Mediator.Notifications;
using Tawtheef.Domain.Events.Operation.Employee.JobCandidates;

namespace Application.Operation.Common.Events.JobCandidates;

public sealed class JobCandidateInvitationSentDomainEventHandler
    : INotificationHandler<JobCandidateInvitationSentDomainEvent>
{
    public async Task Handle(JobCandidateInvitationSentDomainEvent notification, CancellationToken ct)
    {
        var jobTitle = notification.JobTitle;
        var body = string.IsNullOrWhiteSpace(jobTitle)
            ? "You have been invited to apply for a job on Tawtheef."
            : $"You have been invited to apply for {jobTitle} on Tawtheef.";

        if (!string.IsNullOrWhiteSpace(notification.Email))
        {
            //TODO: should be add to notification table
            await emailSender.SendAsync(
                notification.Email,
                "Job Invitation",
                body,
                ct);
        }

        if (!string.IsNullOrWhiteSpace(notification.PhoneNumber))
        {
            //TODO: should be add to notification table
            await smsSender.SendAsync(
                notification.PhoneNumber,
                body,
                ct);
        }
    }
}
