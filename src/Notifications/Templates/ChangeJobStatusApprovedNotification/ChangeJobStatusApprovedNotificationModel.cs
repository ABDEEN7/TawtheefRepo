using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ChangeJobStatusApprovedNotification;

public static class ChangeJobStatusApprovedNotification
{
    public const string TemplateKey = nameof(ChangeJobStatusApprovedNotification);
}

[NotificationTemplate(ChangeJobStatusApprovedNotification.TemplateKey, "تم اعتماد الوظيفة", "The job has been approved")]
public sealed record ChangeJobStatusApprovedNotificationModel(string JobTitle);
