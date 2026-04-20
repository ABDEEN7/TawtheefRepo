using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.JobUpdatedNotification;

public static class JobUpdatedNotification
{
    public const string TemplateKey = nameof(JobUpdatedNotification);
}

[NotificationTemplate(JobUpdatedNotification.TemplateKey, "تم تحديث الوظيفة", "Job Posting Updated")]
public sealed record JobUpdatedNotificationModel(string JobTitle);
