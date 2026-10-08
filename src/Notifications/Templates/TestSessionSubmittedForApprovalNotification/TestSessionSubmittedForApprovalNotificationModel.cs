using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.TestSessionSubmittedForApprovalNotification;

public static class TestSessionSubmittedForApprovalNotification
{
    public const string TemplateKey = nameof(TestSessionSubmittedForApprovalNotification);
}

[NotificationTemplate(TestSessionSubmittedForApprovalNotification.TemplateKey, "إرسال جلسة الاختبار للاعتماد", "Test session submitted for approval")]
public sealed record TestSessionSubmittedForApprovalNotificationModel(string SessionNo);
