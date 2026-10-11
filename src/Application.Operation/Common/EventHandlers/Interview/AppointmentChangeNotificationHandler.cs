using Application.Operation.Common.Services.Interview;
using MediatR;
using Microsoft.Extensions.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.Operation.Employee.Interview.Appointment;
using Tawtheef.Notifications.Templates.InterviewAppointmentChanged;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Application.Operation.Common.EventHandlers.Interview;

/// <summary>
/// Tells an invited candidate that their interview was rescheduled or cancelled (individually, or with its
/// whole schedule) by bell, email and SMS. A candidate who was never invited never heard of the old time,
/// so they are not told (a rescheduled one is invited for the new time as usual). The staff-entered reason
/// is internal and is not passed on.
/// </summary>
public sealed class AppointmentChangeNotificationHandler(
    IUnitOfWork unitOfWork,
    IInterviewNotifier notifier,
    ILogger<AppointmentChangeNotificationHandler> logger)
    : INotificationHandler<AppointmentRescheduledEvent>,
        INotificationHandler<AppointmentCancelledEvent>
{
    public Task Handle(AppointmentRescheduledEvent notification, CancellationToken ct) =>
        NotifyAsync(InterviewAppointmentChange.Rescheduled, notification.OldAppointment, notification.NewAppointment, ct);

    public Task Handle(AppointmentCancelledEvent notification, CancellationToken ct) =>
        NotifyAsync(InterviewAppointmentChange.Cancelled, notification.Appointment, null, ct);

    private async Task NotifyAsync(
        InterviewAppointmentChange change,
        InterviewAppointment oldAppointment,
        InterviewAppointment? newAppointment,
        CancellationToken ct)
    {
        if (oldAppointment.InvitationSentAt is null) return;

        try
        {
            var candidate = await InterviewCandidate.LoadAsync(unitOfWork, oldAppointment.InvitationId, ct);
            if (candidate is null) return;

            var old = await InterviewAppointmentSlot.LoadAsync(unitOfWork, oldAppointment, ct);
            var updated = newAppointment is null ? null : await InterviewAppointmentSlot.LoadAsync(unitOfWork, newAppointment, ct);

            await notifier.QueueAsync(
                candidate.Applicant,
                InterviewAppointmentChanged.TemplateKey,
                lang => new InterviewAppointmentChangedModel(
                    change,
                    InterviewNotificationText.FullName(candidate.Applicant, lang) ?? "",
                    candidate.Job(lang),
                    old.Day(lang),
                    old.Date,
                    old.Start(lang),
                    old.End(lang),
                    updated?.Day(lang),
                    updated?.Date,
                    updated?.Start(lang),
                    updated?.End(lang),
                    updated?.IsRemote ?? false,
                    updated?.Location(lang),
                    updated?.LocationMapUrl,
                    updated?.MeetingUrl,
                    updated?.Instructions),
                lang => InterviewNotificationText.AppointmentChangeSubject(change, candidate.Job(lang), lang),
                [NotificationChannel.InApp, NotificationChannel.Email, NotificationChannel.Sms],
                ct);

            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // A notification failure must not undo the reschedule / cancellation itself.
            logger.LogError(ex, "Failed to queue interview {Change} notice for appointment {AppointmentId}",
                change, oldAppointment.Id);
        }
    }
}
