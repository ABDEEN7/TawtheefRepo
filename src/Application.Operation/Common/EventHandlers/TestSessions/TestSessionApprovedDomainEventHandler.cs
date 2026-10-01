using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.TestSessions;
using Tawtheef.Notifications.Templates.TestSessionApprovedNotification;
using Tawtheef.Notifications.Templates.TestSessionStaffAssignedNotification;

namespace Application.Operation.Common.EventHandlers.TestSessions;

public sealed class TestSessionApprovedDomainEventHandler(UserManager<User> userManager, IUnitOfWork unitOfWork)
    : INotificationHandler<TestSessionApprovedDomainEvent>
{
    public async Task Handle(TestSessionApprovedDomainEvent notification, CancellationToken ct)
    {
        var creator = await userManager.FindByIdAsync(notification.CreatorId.ToString());
        var sessionDetails = await unitOfWork.Context.Set<TestSession>().AsNoTracking()
            .Where(session => session.Id == notification.TestSessionId)
            .Select(session => new
            {
                ExamNameAr = session.Exam!.TitleAr,
                ExamNameEn = session.Exam!.TitleEn,
                JobTitleAr = session.Exam!.Job!.JobTitle!.JobNameAr,
                JobTitleEn = session.Exam!.Job!.JobTitle!.JobNameEn,
                TestDate = session.TestSlot!.SlotDate,
                session.StartTime,
                session.EndTime,
                TestSlotId = session.TestSlot!.Id,
            })
            .FirstOrDefaultAsync(ct);

        var staffUserIds = sessionDetails is null
            ? new List<Guid>()
            : await unitOfWork.Context.Set<TestSlotStaff>().AsNoTracking()
                .Where(staff => staff.TestSlotId == sessionDetails.TestSlotId && staff.IsActive)
                .Select(staff => staff.StaffUserId)
                .Distinct()
                .ToListAsync(ct);

        if (creator is not null)
        {
            var inAppNotification = Notification.Create(NotificationChannel.InApp,
                TestSessionApprovedNotification.TemplateKey, creator.Id, creator.Email,
                payloadJson: JsonSerializer.Serialize(new TestSessionApprovedNotificationModel(notification.SessionNo)),
                language: creator.GetPreferredLanguage(),
                idempotencyKey: $"test-session-approved-{notification.TestSessionId:N}-{notification.DateOccurred.UtcDateTime.Ticks}");
            await unitOfWork.GetEntityRepository<Notification>().AddAsync(inAppNotification, ct);
        }

        if (sessionDetails is not null)
        {
            foreach (var staffUserId in staffUserIds)
            {
                var staffUser = await userManager.FindByIdAsync(staffUserId.ToString());
                if (staffUser is null) continue;

                var isArabic = staffUser.GetPreferredLanguage().StartsWith("ar", StringComparison.OrdinalIgnoreCase);
                var staffModel = new TestSessionStaffAssignedNotificationModel(
                    isArabic ? sessionDetails.ExamNameAr : sessionDetails.ExamNameEn ?? sessionDetails.ExamNameAr,
                    isArabic ? sessionDetails.JobTitleAr : sessionDetails.JobTitleEn,
                    sessionDetails.TestDate,
                    sessionDetails.StartTime!.Value,
                    sessionDetails.EndTime!.Value);
                var staffNotification = Notification.Create(NotificationChannel.InApp,
                    TestSessionStaffAssignedNotification.TemplateKey, staffUser.Id, staffUser.Email,
                    payloadJson: JsonSerializer.Serialize(staffModel),
                    language: staffUser.GetPreferredLanguage(),
                    idempotencyKey: $"tss-{notification.TestSessionId:N}-{staffUser.Id:N}-" +
                                    notification.DateOccurred.UtcDateTime.Ticks);
                await unitOfWork.GetEntityRepository<Notification>().AddAsync(staffNotification, ct);
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}
