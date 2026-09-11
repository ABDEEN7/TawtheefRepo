using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ExamApprovedNotification;

public static class ExamApprovedNotification
{
    public const string TemplateKey = nameof(ExamApprovedNotification);
}

[NotificationTemplate(ExamApprovedNotification.TemplateKey, "تم اعتماد الاختبار", "The exam has been approved")]
public sealed record ExamApprovedNotificationModel(string ExamNumber);
