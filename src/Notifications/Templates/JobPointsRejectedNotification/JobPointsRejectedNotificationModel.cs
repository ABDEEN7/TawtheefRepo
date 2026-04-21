using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.JobPointsRejectedNotification;

public static class JobPointsRejectedNotification
{
    public const string TemplateKey = nameof(JobPointsRejectedNotification);
}

[NotificationTemplate(JobPointsRejectedNotification.TemplateKey, "إرجاع نقاط الوظيفة للتعديل", "Job Points Returned for Updates")]
public sealed record JobPointsRejectedNotificationModel(string JobTitle, string Reason);
