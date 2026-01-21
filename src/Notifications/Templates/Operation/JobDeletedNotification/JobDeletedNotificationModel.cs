using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.JobDeletedNotification;

public static class JobDeletedNotification
{
    public const string TemplateKey = nameof(JobDeletedNotification);
}

[NotificationTemplate(JobDeletedNotification.TemplateKey)]
public sealed record JobDeletedNotificationModel(string JobTitle);
