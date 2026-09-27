using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Exams;
using Tawtheef.Notifications.Templates.CandidateExamRescheduledNotification;

namespace Application.Operation.Common.EventHandlers.Exams;

public sealed class CandidateExamRescheduledDomainEventHandler(
    UserManager<User> userManager, IUnitOfWork unitOfWork)
    : INotificationHandler<CandidateExamRescheduledDomainEvent>
{
    public async Task Handle(CandidateExamRescheduledDomainEvent notification, CancellationToken ct)
    {
        var applicant = await userManager.FindByIdAsync(notification.ApplicantId.ToString());
        if (applicant is null)
            throw new InvalidOperationException($"Applicant {notification.ApplicantId} is missing for rescheduled exam assignment.");

        // Candidate messages contain appointment details only; exam results remain internal.
        var payload = JsonSerializer.Serialize(new CandidateExamRescheduledNotificationModel(
            notification.ExamTitleAr, notification.ExamTitleEn,
            notification.SlotDate, notification.StartTime,
            notification.LocationAr, notification.LocationEn,
            notification.RoomAr, notification.RoomEn, notification.SessionNo));
        await unitOfWork.GetEntityRepository<Notification>().AddAsync(Notification.Create(
            NotificationChannel.InApp,
            CandidateExamRescheduledNotification.TemplateKey,
            applicant.Id,
            applicant.Email,
            payloadJson: payload,
            language: applicant.GetPreferredLanguage(),
            idempotencyKey: $"exam-rescheduled-{notification.AssignmentId:N}-{applicant.Id:N}"), ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
