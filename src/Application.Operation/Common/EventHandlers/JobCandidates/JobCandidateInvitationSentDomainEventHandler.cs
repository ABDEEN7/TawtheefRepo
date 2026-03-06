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
{
    public async Task Handle(JobCandidateInvitationSentDomainEvent request, CancellationToken ct)
    {
        var repo = unitOfWork.GetEntityRepository<Notification>();
        var jobTitle = request.JobTitle;

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var payload = JsonSerializer.Serialize(new JobCandidateInvitationSentModel(jobTitle));
            var emailNotification = Notification.Create( NotificationChannel.Email, 
                JobCandidateInvitationSent.TemplateKey, request.ApplicantId, 
                request.Email, "Careers Job Invitation", null, payload);
            await repo.AddAsync(emailNotification, ct);
        }
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var body = string.IsNullOrWhiteSpace(jobTitle)
                ? JobCandidatesMessages.JobInvitationWithoutTitle
                : string.Format(JobCandidatesMessages.JobInvitationWithTitle, jobTitle);

            var smsNotification = Notification.Create( NotificationChannel.Sms, 
                JobCandidateInvitationSent.TemplateKey, request.ApplicantId, 
                request.PhoneNumber, "Careers Job Invitation", body, null);
            await repo.AddAsync(smsNotification, ct);
        }
        
        var notificationInApp = Notification.Create(
            NotificationChannel.InApp,
            JobCandidateInvitationSent.TemplateKey,
            request.ApplicantId,
            request.Email,
            "Careers Job Invitation",
            string.IsNullOrWhiteSpace(jobTitle)
                ? JobCandidatesMessages.JobInvitationWithoutTitle
                : string.Format(JobCandidatesMessages.JobInvitationWithTitle, jobTitle),
            null);
        await repo.AddAsync(notificationInApp, ct);
        

        await unitOfWork.SaveChangesAsync(ct);
    }
}

