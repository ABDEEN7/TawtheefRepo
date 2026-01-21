using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.JobUpdatedNotification;

public static class JobUpdatedNotification
{
    public const string TemplateKey = nameof(JobUpdatedNotification);
}

[NotificationTemplate(JobUpdatedNotification.TemplateKey)]
public sealed record JobUpdatedNotificationModel(string JobTitle);
