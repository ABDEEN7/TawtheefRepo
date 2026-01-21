using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ChangeJobStatusNeedUpdateNotification;

public static class ChangeJobStatusNeedUpdateNotification
{
    public const string TemplateKey = nameof(ChangeJobStatusNeedUpdateNotification);
}

[NotificationTemplate(ChangeJobStatusNeedUpdateNotification.TemplateKey)]
public sealed record ChangeJobStatusNeedUpdateNotificationModel(string JobTitle);
