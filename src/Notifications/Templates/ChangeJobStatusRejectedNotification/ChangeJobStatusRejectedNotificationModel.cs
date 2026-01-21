using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ChangeJobStatusRejectedNotification;

public static class ChangeJobStatusRejectedNotification
{
    public const string TemplateKey = nameof(ChangeJobStatusRejectedNotification);
}

[NotificationTemplate(ChangeJobStatusRejectedNotification.TemplateKey)]
public sealed record ChangeJobStatusRejectedNotificationModel(string JobTitle);
