using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ExamRejectedNotification;

public static class ExamRejectedNotification
{
    public const string TemplateKey = nameof(ExamRejectedNotification);
}

[NotificationTemplate(ExamRejectedNotification.TemplateKey, "رفض الاختبار", "Exam Rejected")]
public sealed record ExamRejectedNotificationModel(string ExamNumber, string Note);
