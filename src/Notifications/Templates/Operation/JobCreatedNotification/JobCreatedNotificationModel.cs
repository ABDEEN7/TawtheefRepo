using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.JobCreatedNotification;

public sealed record JobCreatedNotification;

[NotificationTemplate(nameof(JobCreatedNotification))]
public sealed record JobCreatedNotificationModel(string JobTitle);
