using Tawtheef.Notifications.Attributes;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Tawtheef.Notifications.Templates.InterviewAppointmentChanged;

public static class InterviewAppointmentChanged
{
    public const string TemplateKey = nameof(InterviewAppointmentChanged);
}

/// <summary>
/// Tells an invited candidate that their interview moved or was cancelled. Wording is generic on purpose:
/// the staff-entered reason is internal and is never shown. Dates/times are pre-formatted in Qatar local
/// time in the recipient's language.
/// </summary>
/// <param name="OldDay">The interview the candidate was invited to: weekday name.</param>
/// <param name="NewDay">Rescheduled only: the new interview's weekday name (New* and the place fields describe it).</param>
/// <param name="Location">Rescheduled, in person only: room and building of the new interview.</param>
/// <param name="MeetingUrl">Rescheduled, remote only.</param>
[NotificationTemplate(InterviewAppointmentChanged.TemplateKey, "تحديث على موعد مقابلتكم", "Update on your interview")]
public sealed record InterviewAppointmentChangedModel(
    InterviewAppointmentChange Change,
    string CandidateName,
    string JobTitle,
    string OldDay,
    string OldDate,
    string OldStartTime,
    string OldEndTime,
    string? NewDay,
    string? NewDate,
    string? NewStartTime,
    string? NewEndTime,
    bool IsRemote,
    string? Location,
    string? LocationMapUrl,
    string? MeetingUrl,
    string? Instructions);
