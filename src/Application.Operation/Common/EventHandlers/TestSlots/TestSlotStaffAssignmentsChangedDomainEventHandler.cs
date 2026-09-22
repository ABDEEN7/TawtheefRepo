using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.TestSlots;
using Tawtheef.Notifications.Templates.TestSlotStaffAssignedNotification;
using Tawtheef.Notifications.Templates.TestSlotStaffUnassignedNotification;

namespace Application.Operation.Common.EventHandlers.TestSlots;

public sealed class TestSlotStaffAssignmentsChangedDomainEventHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork) : INotificationHandler<TestSlotStaffAssignmentsChangedDomainEvent>
{
    public async Task Handle(TestSlotStaffAssignmentsChangedDomainEvent notification, CancellationToken ct)
    {
        foreach (var change in notification.Changes)
        {
            var staffUser = await userManager.FindByIdAsync(change.StaffUserId.ToString());
            if (staffUser is null)
                continue;

            var (templateKey, payload, idempotencyPrefix) = change.Type switch
            {
                TestSlotStaffAssignmentNotificationType.Assigned => (
                    TestSlotStaffAssignedNotification.TemplateKey,
                    JsonSerializer.Serialize(new TestSlotStaffAssignedNotificationModel(
                        change.StaffUserId, notification.TestSlotId, notification.TestSlotTitleAr,
                        notification.TestSlotTitleEn, notification.SlotDate, change.RoleId)),
                    "ts-a"),
                TestSlotStaffAssignmentNotificationType.Unassigned => (
                    TestSlotStaffUnassignedNotification.TemplateKey,
                    JsonSerializer.Serialize(new TestSlotStaffUnassignedNotificationModel(
                        change.StaffUserId, notification.TestSlotId, notification.TestSlotTitleAr,
                        notification.TestSlotTitleEn, notification.SlotDate, change.RoleId)),
                    "ts-u"),
                _ => throw new ArgumentOutOfRangeException(nameof(change.Type), change.Type, null),
            };

            var inAppNotification = Notification.Create(
                NotificationChannel.InApp,
                templateKey,
                staffUser.Id,
                staffUser.Email,
                payloadJson: payload,
                language: staffUser.GetPreferredLanguage(),
                idempotencyKey: $"{idempotencyPrefix}-{notification.TestSlotId:N}-{staffUser.Id:N}-" +
                                notification.DateOccurred.UtcDateTime.Ticks);
            await unitOfWork.GetEntityRepository<Notification>().AddAsync(inAppNotification, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}
