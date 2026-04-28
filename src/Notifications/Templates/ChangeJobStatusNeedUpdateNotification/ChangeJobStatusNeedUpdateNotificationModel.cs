using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ChangeJobStatusNeedUpdateNotification;

public static class ChangeJobStatusNeedUpdateNotification
{
    public const string TemplateKey = nameof(ChangeJobStatusNeedUpdateNotification);
}

[NotificationTemplate(ChangeJobStatusNeedUpdateNotification.TemplateKey, "تم إرجاع الوظيفة للتعديل", "Job Returned for Update")]
public sealed record ChangeJobStatusNeedUpdateNotificationModel(string JobTitle);
