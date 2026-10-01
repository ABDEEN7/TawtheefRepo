using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.TestSessionApprovedNotification;

public static class TestSessionApprovedNotification
{
    public const string TemplateKey = nameof(TestSessionApprovedNotification);
}

[NotificationTemplate(TestSessionApprovedNotification.TemplateKey, "تم اعتماد جلسة الاختبار", "Test session approved")]
public sealed record TestSessionApprovedNotificationModel(string SessionNo);
