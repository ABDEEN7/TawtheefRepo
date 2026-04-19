using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ChangeJobStatusNeedUpdateNotification;

public static class ChangeJobStatusNeedUpdateNotification
{
    public const string TemplateKey = nameof(ChangeJobStatusNeedUpdateNotification);
}

[NotificationTemplate(ChangeJobStatusNeedUpdateNotification.TemplateKey, "طلب الوظيفة بحاجة لتحديث", "Job Application Needs Updates")]
public sealed record ChangeJobStatusNeedUpdateNotificationModel(string JobTitle);
