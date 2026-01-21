using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.OfficeCreatedNotification;

public static class OfficeCreatedNotification
{
    public const string TemplateKey = nameof(OfficeCreatedNotification);
}

[NotificationTemplate(OfficeCreatedNotification.TemplateKey)]
public sealed record OfficeCreatedNotificationModel(string OfficeName, string OrganizationName, string PortalUrl);
