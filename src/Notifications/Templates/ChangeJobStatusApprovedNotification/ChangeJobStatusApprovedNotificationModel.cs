using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ChangeJobStatusApprovedNotification;

public static class ChangeJobStatusApprovedNotification
{
    public const string TemplateKey = nameof(ChangeJobStatusApprovedNotification);
}

[NotificationTemplate(ChangeJobStatusApprovedNotification.TemplateKey, "تمت الموافقة على طلب الوظيفة", "Job Approval Confirmed")]
public sealed record ChangeJobStatusApprovedNotificationModel(string JobTitle);
