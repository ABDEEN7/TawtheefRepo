using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.ChangeJobStatusNotification;

public static class ChangeJobStatusNotification
{
    public const string TemplateKey = nameof(ChangeJobStatusNotification);
}

[NotificationTemplate(ChangeJobStatusNotification.TemplateKey)]
public sealed record ChangeJobStatusNotificationModel(string JobTitle);
