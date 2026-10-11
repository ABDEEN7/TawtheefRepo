using Tawtheef.Notifications.Attributes;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Tawtheef.Notifications.Templates.InterviewCommitteeAssignment;

public static class InterviewCommitteeAssignment
{
    public const string TemplateKey = nameof(InterviewCommitteeAssignment);

    /// <summary>The page a member lands on when the payload carries no usable LinkPath.</summary>
    public static string DefaultPath(InterviewAssignmentChange change) =>
        change is InterviewAssignmentChange.ScheduleApproved or InterviewAssignmentChange.ScheduleCancelled
            ? InterviewNotificationLinks.StartInterviewPath
            : InterviewNotificationLinks.DefaultPath(InterviewEntityKind.Committee);
}

/// <param name="Role">The member's committee role, already translated.</param>
/// <param name="ScheduleName">ScheduleApproved / ScheduleCancelled only.</param>
/// <param name="FromDate">ScheduleApproved / ScheduleCancelled only: first interview day, pre-formatted in Qatar local time.</param>
/// <param name="ToDate">ScheduleApproved / ScheduleCancelled only: last interview day, pre-formatted in Qatar local time.</param>
/// <param name="InterviewCount">ScheduleApproved / ScheduleCancelled only: interviews of this committee in the schedule.</param>
/// <param name="LinkPath">Relative operations portal path.</param>
/// <param name="Reason">ScheduleCancelled only: the cancellation reason.</param>
[NotificationTemplate(InterviewCommitteeAssignment.TemplateKey, "تحديث على عضويتكم في لجنة المقابلة", "Update on your interview committee membership")]
public sealed record InterviewCommitteeAssignmentModel(
    InterviewAssignmentChange Change,
    string CommitteeName,
    string? JobTitle,
    string? Role,
    string? ScheduleName,
    string? FromDate,
    string? ToDate,
    int? InterviewCount,
    string LinkPath,
    string? Reason = null);
