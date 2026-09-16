using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Exams;
using Tawtheef.Notifications.Templates.ExamRejectedNotification;

namespace Application.Operation.Common.EventHandlers.Exams;

public sealed class ExamRejectedDomainEventHandler(UserManager<User> userManager, IUnitOfWork unitOfWork)
    : INotificationHandler<ExamRejectedDomainEvent>
{
    public async Task Handle(ExamRejectedDomainEvent notification, CancellationToken ct)
    {
        var creator = await userManager.FindByIdAsync(notification.CreatorId.ToString());
        if (creator == null)
            return;

        var payload = JsonSerializer.Serialize(new ExamRejectedNotificationModel(notification.ExamNumber, notification.Note));
        var inAppNotification = Notification.Create(
            NotificationChannel.InApp,
            ExamRejectedNotification.TemplateKey,
            creator.Id,
            creator.Email,
            payloadJson: payload,
            language: creator.GetPreferredLanguage(),
            idempotencyKey: $"exam-rejected-{notification.ExamId:N}-{notification.DateOccurred.UtcDateTime.Ticks}");
        await unitOfWork.GetEntityRepository<Notification>().AddAsync(inAppNotification, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
