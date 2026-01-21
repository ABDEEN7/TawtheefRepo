using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.ChangeJobStatusApprovedNotification;

public sealed record ChangeJobStatusApprovedNotification;

[NotificationTemplate(nameof(ChangeJobStatusApprovedNotification))]
public sealed record ChangeJobStatusApprovedNotificationModel(string JobTitle);
