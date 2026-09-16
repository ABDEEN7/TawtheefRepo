using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ExamReturnedForEditNotification;

public static class ExamReturnedForEditNotification
{
    public const string TemplateKey = nameof(ExamReturnedForEditNotification);
}

[NotificationTemplate(ExamReturnedForEditNotification.TemplateKey, "إرجاع الاختبار للتعديل", "Exam Returned for Editing")]
public sealed record ExamReturnedForEditNotificationModel(string ExamNumber, string Note);
