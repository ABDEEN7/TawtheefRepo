using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.OfficeCreatedNotification;

public static class OfficeCreatedNotification
{
    public const string TemplateKey = nameof(OfficeCreatedNotification);
}

[NotificationTemplate(OfficeCreatedNotification.TemplateKey, "تم إنشاء مكتب جديد", "New Recruitment Office Created")]
public sealed record OfficeCreatedNotificationModel(string OfficeNameAr, string OfficeNameEn, string OfficeCode, string AdminEmail);
