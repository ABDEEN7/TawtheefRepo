using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.TestSessionStaffAssignedNotification;

public static class TestSessionStaffAssignedNotification
{
    public const string TemplateKey = nameof(TestSessionStaffAssignedNotification);
}

[NotificationTemplate(TestSessionStaffAssignedNotification.TemplateKey, "إضافة جلسة اختبار", "Test session scheduled")]
public sealed record TestSessionStaffAssignedNotificationModel(
    string ExamName,
    string JobTitle,
    DateOnly TestDate,
    TimeOnly StartTime,
    TimeOnly EndTime);
