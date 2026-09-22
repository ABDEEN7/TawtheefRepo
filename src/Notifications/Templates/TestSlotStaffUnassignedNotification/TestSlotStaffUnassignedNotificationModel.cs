using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.TestSlotStaffUnassignedNotification;

public static class TestSlotStaffUnassignedNotification
{
    public const string TemplateKey = nameof(TestSlotStaffUnassignedNotification);
}

[NotificationTemplate(TestSlotStaffUnassignedNotification.TemplateKey, "إلغاء تكليف فترة اختبار", "Test Period Assignment Cancelled")]
public sealed record TestSlotStaffUnassignedNotificationModel(
    Guid StaffUserId,
    Guid TestSlotId,
    string TestSlotTitleAr,
    string? TestSlotTitleEn,
    DateOnly SlotDate,
    Guid RoleId);
