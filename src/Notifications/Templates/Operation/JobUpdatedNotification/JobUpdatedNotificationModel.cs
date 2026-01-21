using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.JobUpdatedNotification;

public sealed record JobUpdatedNotification;

[NotificationTemplate(nameof(JobUpdatedNotification))]
public sealed record JobUpdatedNotificationModel(string JobTitle);
