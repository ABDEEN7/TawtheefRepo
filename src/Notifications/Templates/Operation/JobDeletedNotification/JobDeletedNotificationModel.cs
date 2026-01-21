using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.JobDeletedNotification;

public sealed record JobDeletedNotification;

[NotificationTemplate(nameof(JobDeletedNotification))]
public sealed record JobDeletedNotificationModel(string JobTitle);
