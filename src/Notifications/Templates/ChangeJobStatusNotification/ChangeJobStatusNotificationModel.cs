using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ChangeJobStatusNotification;

public static class ChangeJobStatusNotification
{
    public const string TemplateKey = nameof(ChangeJobStatusNotification);
}

[NotificationTemplate(ChangeJobStatusNotification.TemplateKey, "تحديث حالة طلب الوظيفة", "Job Application Status Updated")]
public sealed record ChangeJobStatusNotificationModel(string JobTitle);
