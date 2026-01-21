using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.ChangeJobStatusNotification;

public sealed record ChangeJobStatusNotification;

[NotificationTemplate(nameof(ChangeJobStatusNotification))]
public sealed record ChangeJobStatusNotificationModel(string JobTitle);
