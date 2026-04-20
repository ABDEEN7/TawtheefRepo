using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.JobDeletedNotification;

public static class JobDeletedNotification
{
    public const string TemplateKey = nameof(JobDeletedNotification);
}

[NotificationTemplate(JobDeletedNotification.TemplateKey, "تم حذف الوظيفة", "Job Posting Removed")]
public sealed record JobDeletedNotificationModel(string JobTitle);
