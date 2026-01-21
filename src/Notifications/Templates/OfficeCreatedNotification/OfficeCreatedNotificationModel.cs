using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.OfficeCreatedNotification;

public static class OfficeCreatedNotification
{
    public const string TemplateKey = nameof(OfficeCreatedNotification);
}

[NotificationTemplate(OfficeCreatedNotification.TemplateKey)]
public sealed record OfficeCreatedNotificationModel(string OfficeName, string OfficeCode, string AdminEmail);
