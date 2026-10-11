using Application.Operation.Common.Services.Interview;
using MediatR;
using Microsoft.Extensions.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.Operation.Employee.Interview.Appointment;
using Tawtheef.Notifications.Templates.InterviewInvitation;

namespace Application.Operation.Common.EventHandlers.Interview;

/// <summary>
/// Sends the candidate their interview invitation (first send) or a reminder (later sends) by bell, email
/// and SMS, in the candidate's language with Qatar-local times. Invitations are sent manually by staff
/// (per appointment or "send to all"), never automatically on schedule approval.
/// </summary>
public sealed class AppointmentInvitationNotificationHandler(
    IUnitOfWork unitOfWork,
    IInterviewNotifier notifier,
    ILogger<AppointmentInvitationNotificationHandler> logger)
    : INotificationHandler<AppointmentNotificationSentEvent>
{
    public async Task Handle(AppointmentNotificationSentEvent notification, CancellationToken ct)
    {
        var appointment = notification.Appointment;
        try
        {
            var candidate = await InterviewCandidate.LoadAsync(unitOfWork, appointment.InvitationId, ct);
            if (candidate is null) return;

            var slot = await InterviewAppointmentSlot.LoadAsync(unitOfWork, appointment, ct);

            await notifier.QueueAsync(
                candidate.Applicant,
                InterviewInvitation.TemplateKey,
                lang => new InterviewInvitationModel(
                    notification.IsReminder,
                    InterviewNotificationText.FullName(candidate.Applicant, lang) ?? "",
                    candidate.Job(lang),
                    slot.Day(lang),
                    slot.Date,
                    slot.Start(lang),
                    slot.End(lang),
                    slot.IsRemote,
                    slot.Location(lang),
                    slot.LocationMapUrl,
                    slot.MeetingUrl,
                    slot.Instructions),
                lang => InterviewNotificationText.InvitationSubject(notification.IsReminder, candidate.Job(lang), lang),
                [NotificationChannel.InApp, NotificationChannel.Email, NotificationChannel.Sms],
                ct);

            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // A notification failure must not undo the send stamp itself; staff can send a reminder again.
            logger.LogError(ex, "Failed to queue interview {Kind} for appointment {AppointmentId}",
                notification.IsReminder ? "reminder" : "invitation", appointment.Id);
        }
    }
}
