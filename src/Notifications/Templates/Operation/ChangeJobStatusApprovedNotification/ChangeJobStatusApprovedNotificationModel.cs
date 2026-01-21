using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.ChangeJobStatusApprovedNotification;

public static class ChangeJobStatusApprovedNotification
{
    public const string TemplateKey = nameof(ChangeJobStatusApprovedNotification);
}

[NotificationTemplate(ChangeJobStatusApprovedNotification.TemplateKey)]
public sealed record ChangeJobStatusApprovedNotificationModel(string JobTitle);
