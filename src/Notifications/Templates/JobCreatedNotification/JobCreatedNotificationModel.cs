using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.JobCreatedNotification;

public static class JobCreatedNotification
{
    public const string TemplateKey = nameof(JobCreatedNotification);
}

[NotificationTemplate(JobCreatedNotification.TemplateKey, "تم إنشاء وظيفة جديدة", "New Job Posting Created")]
public sealed record JobCreatedNotificationModel(string JobTitle);
