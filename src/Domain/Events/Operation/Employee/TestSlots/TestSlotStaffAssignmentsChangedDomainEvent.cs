using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.TestSlots;

public sealed record TestSlotStaffAssignmentsChangedDomainEvent(
    Guid TestSlotId,
    string TestSlotTitleAr,
    string? TestSlotTitleEn,
    DateOnly SlotDate,
    IReadOnlyCollection<TestSlotStaffAssignmentNotification> Changes) : BaseEvent(DateTimeOffset.UtcNow);

public sealed record TestSlotStaffAssignmentNotification(
    Guid StaffUserId,
    Guid RoleId,
    TestSlotStaffAssignmentNotificationType Type);

public enum TestSlotStaffAssignmentNotificationType
{
    Assigned,
    Unassigned,
}
