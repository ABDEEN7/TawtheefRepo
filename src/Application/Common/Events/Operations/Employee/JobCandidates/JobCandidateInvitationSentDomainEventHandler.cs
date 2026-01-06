using MediatR;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Events.Operation.Employee.JobCandidates;

namespace Tawtheef.Application.Common.Events.Operations.Employee.JobCandidates;

public sealed class JobCandidateInvitationSentDomainEventHandler(
    IEmailSender emailSender,
    ISmsSender smsSender)
    : INotificationHandler<JobCandidateInvitationSentDomainEvent>
{
    public async Task Handle(JobCandidateInvitationSentDomainEvent notification, CancellationToken ct)
    {
        if (notification is null)
        {
            return;
        }

        var jobTitle = notification.JobTitle;
        var body = string.IsNullOrWhiteSpace(jobTitle)
            ? "You have been invited to apply for a job on Tawtheef."
            : $"You have been invited to apply for {jobTitle} on Tawtheef.";

        if (!string.IsNullOrWhiteSpace(notification.Email))
        {
            await emailSender.SendAsync(
                notification.Email,
                "Job Invitation",
                body,
                ct);
        }

        if (!string.IsNullOrWhiteSpace(notification.PhoneNumber))
        {
            await smsSender.SendAsync(
                notification.PhoneNumber,
                body,
                ct);
        }
    }
}
