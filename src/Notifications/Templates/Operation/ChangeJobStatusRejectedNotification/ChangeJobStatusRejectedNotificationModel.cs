using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.ChangeJobStatusRejectedNotification;

public sealed record ChangeJobStatusRejectedNotification;

[NotificationTemplate(nameof(ChangeJobStatusRejectedNotification))]
public sealed record ChangeJobStatusRejectedNotificationModel(string JobTitle);
