using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.InterviewInvitation;

public static class InterviewInvitation
{
    public const string TemplateKey = nameof(InterviewInvitation);
}

/// <summary>
/// The candidate's interview invitation (first send) or reminder (later sends). Every date/time value is
/// pre-formatted in Qatar local time in the recipient's language: appointment times are stored in UTC and
/// the template has no time zone.
/// </summary>
/// <param name="InterviewDay">Weekday name, e.g. "الأحد" / "Sunday".</param>
/// <param name="InterviewDate">dd/MM/yyyy.</param>
/// <param name="Location">In-person only: room and building.</param>
/// <param name="LocationMapUrl">In-person only: the building's map link, when it has one.</param>
/// <param name="MeetingUrl">Remote only.</param>
[NotificationTemplate(InterviewInvitation.TemplateKey, "دعوة لحضور مقابلة شخصية", "Interview invitation")]
public sealed record InterviewInvitationModel(
    bool IsReminder,
    string CandidateName,
    string JobTitle,
    string InterviewDay,
    string InterviewDate,
    string StartTime,
    string EndTime,
    bool IsRemote,
    string? Location,
    string? LocationMapUrl,
    string? MeetingUrl,
    string? Instructions);
