using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Exams;
using Tawtheef.Notifications.Templates.ExamReturnedForEditNotification;

namespace Application.Operation.Common.EventHandlers.Exams;

public sealed class ExamReturnedForEditDomainEventHandler(UserManager<User> userManager, IUnitOfWork unitOfWork)
    : INotificationHandler<ExamReturnedForEditDomainEvent>
{
    public async Task Handle(ExamReturnedForEditDomainEvent notification, CancellationToken ct)
    {
        var creator = await userManager.FindByIdAsync(notification.CreatorId.ToString());
        if (creator == null)
            return;

        var payload = JsonSerializer.Serialize(new ExamReturnedForEditNotificationModel(
            notification.ExamNumber, notification.Note));
        var inAppNotification = Notification.Create(
            NotificationChannel.InApp,
            ExamReturnedForEditNotification.TemplateKey,
            creator.Id,
            creator.Email,
            payloadJson: payload,
            language: creator.GetPreferredLanguage(),
            idempotencyKey: $"exam-returned-{notification.ExamId:N}-{notification.DateOccurred.UtcDateTime.Ticks}");
        await unitOfWork.GetEntityRepository<Notification>().AddAsync(inAppNotification, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
