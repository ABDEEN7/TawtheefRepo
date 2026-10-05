using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.TestSessions;
using Tawtheef.Notifications.Templates.TestSessionRejectedNotification;

namespace Application.Operation.Common.EventHandlers.TestSessions;

public sealed class TestSessionRejectedDomainEventHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork) : INotificationHandler<TestSessionRejectedDomainEvent>
{
    public async Task Handle(TestSessionRejectedDomainEvent notification, CancellationToken ct)
    {
        var creator = await userManager.FindByIdAsync(notification.CreatorId.ToString());
        if (creator is null)
            return;

        var payload = JsonSerializer.Serialize(new TestSessionRejectedNotificationModel(
            notification.SessionNo,
            notification.DecisionNote));
        var inAppNotification = Notification.Create(
            NotificationChannel.InApp,
            TestSessionRejectedNotification.TemplateKey,
            creator.Id,
            creator.Email,
            payloadJson: payload,
            language: creator.GetPreferredLanguage(),
            idempotencyKey: $"test-session-rejected-{notification.TestSessionId:N}-{notification.DateOccurred.UtcDateTime.Ticks}");

        await unitOfWork.GetEntityRepository<Notification>().AddAsync(inAppNotification, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
