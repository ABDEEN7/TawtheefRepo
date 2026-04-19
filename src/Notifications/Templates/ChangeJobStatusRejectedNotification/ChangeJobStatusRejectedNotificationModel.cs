using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ChangeJobStatusRejectedNotification;

public static class ChangeJobStatusRejectedNotification
{
    public const string TemplateKey = nameof(ChangeJobStatusRejectedNotification);
}

[NotificationTemplate(ChangeJobStatusRejectedNotification.TemplateKey, "تم رفض طلب الوظيفة", "Job Application Rejected")]
public sealed record ChangeJobStatusRejectedNotificationModel(string JobTitle);
