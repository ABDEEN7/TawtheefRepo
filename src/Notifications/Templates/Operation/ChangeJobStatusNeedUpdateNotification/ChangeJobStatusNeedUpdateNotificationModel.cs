using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.ChangeJobStatusNeedUpdateNotification;

public static class ChangeJobStatusNeedUpdateNotification
{
    public const string TemplateKey = nameof(ChangeJobStatusNeedUpdateNotification);
}

[NotificationTemplate(ChangeJobStatusNeedUpdateNotification.TemplateKey)]
public sealed record ChangeJobStatusNeedUpdateNotificationModel(string JobTitle);
