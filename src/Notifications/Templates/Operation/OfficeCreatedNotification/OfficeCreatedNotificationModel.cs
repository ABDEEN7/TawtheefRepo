using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.OfficeCreatedNotification;

public sealed record OfficeCreatedNotification;

[NotificationTemplate(nameof(OfficeCreatedNotification))]
public sealed record OfficeCreatedNotificationModel(string OfficeName, string OrganizationName, string PortalUrl);
