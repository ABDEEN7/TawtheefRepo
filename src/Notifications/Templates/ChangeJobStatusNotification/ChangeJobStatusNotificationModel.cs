using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ChangeJobStatusNotification;

public static class ChangeJobStatusNotification
{
    public const string TemplateKey = nameof(ChangeJobStatusNotification);
}

[NotificationTemplate(ChangeJobStatusNotification.TemplateKey, "بيانات الوظيفة بانتظار الاعتماد", "Job Details Pending Approval")]
public sealed record ChangeJobStatusNotificationModel(string JobTitle);
