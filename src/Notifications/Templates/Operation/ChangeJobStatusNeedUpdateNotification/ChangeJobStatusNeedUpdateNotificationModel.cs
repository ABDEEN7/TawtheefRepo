using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.ChangeJobStatusNeedUpdateNotification;

public sealed record ChangeJobStatusNeedUpdateNotification;

[NotificationTemplate(nameof(ChangeJobStatusNeedUpdateNotification))]
public sealed record ChangeJobStatusNeedUpdateNotificationModel(string JobTitle);
