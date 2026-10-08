using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.TestSessionRejectedNotification;

public static class TestSessionRejectedNotification
{
    public const string TemplateKey = nameof(TestSessionRejectedNotification);
}

[NotificationTemplate(TestSessionRejectedNotification.TemplateKey, "تم رفض جلسة الاختبار", "Test Session Rejected")]
public sealed record TestSessionRejectedNotificationModel(string SessionNo, string DecisionNote);
