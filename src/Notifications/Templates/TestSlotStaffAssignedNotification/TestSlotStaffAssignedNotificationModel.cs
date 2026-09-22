using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.TestSlotStaffAssignedNotification;

public static class TestSlotStaffAssignedNotification
{
    public const string TemplateKey = nameof(TestSlotStaffAssignedNotification);
}

[NotificationTemplate(TestSlotStaffAssignedNotification.TemplateKey, "تكليف فترة اختبار", "Test Period Assignment")]
public sealed record TestSlotStaffAssignedNotificationModel(
    Guid StaffUserId,
    Guid TestSlotId,
    string TestSlotTitleAr,
    string? TestSlotTitleEn,
    DateOnly SlotDate,
    Guid RoleId);
