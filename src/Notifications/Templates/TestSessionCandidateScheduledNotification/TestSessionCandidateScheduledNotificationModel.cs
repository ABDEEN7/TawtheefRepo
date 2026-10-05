using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.TestSessionCandidateScheduledNotification;

public static class TestSessionCandidateScheduledNotification
{
    public const string TemplateKey = nameof(TestSessionCandidateScheduledNotification);
}

[NotificationTemplate(TestSessionCandidateScheduledNotification.TemplateKey, "تم تحديد موعد الاختبار", "Test Scheduled")]
public sealed record TestSessionCandidateScheduledNotificationModel(
    string JobNameAr,
    string JobNameEn,
    DateOnly TestDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string LocationNameAr,
    string LocationNameEn,
    string RoomNameAr,
    string RoomNameEn,
    string LocationUrl);
