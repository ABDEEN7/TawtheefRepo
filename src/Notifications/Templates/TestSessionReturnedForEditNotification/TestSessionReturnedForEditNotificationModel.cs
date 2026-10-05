using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.TestSessionReturnedForEditNotification;

public static class TestSessionReturnedForEditNotification
{
    public const string TemplateKey = nameof(TestSessionReturnedForEditNotification);
}

[NotificationTemplate(TestSessionReturnedForEditNotification.TemplateKey, "إرجاع جلسة الاختبار للتعديل", "Test session returned for editing")]
public sealed record TestSessionReturnedForEditNotificationModel(string SessionNo, string DecisionNote);
