using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.JobCreatedNotification;

public static class JobCreatedNotification
{
    public const string TemplateKey = nameof(JobCreatedNotification);
}

[NotificationTemplate(JobCreatedNotification.TemplateKey)]
public sealed record JobCreatedNotificationModel(string JobTitle);
