namespace Application.Operation.Templates.OfficeCreatedNotification;

public sealed record OfficeCreatedNotification;
public sealed record OfficeCreatedNotificationModel(string OfficeName, string OfficeCode, string AdminEmail);
