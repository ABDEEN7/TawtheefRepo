using System.Text.Json;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.Operation.Employee.TestSessions;
using Tawtheef.Notifications.Templates.TestSessionSubmittedForApprovalNotification;

namespace Application.Operation.Common.EventHandlers.TestSessions;

public sealed class TestSessionSubmittedForApprovalDomainEventHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : INotificationHandler<TestSessionSubmittedForApprovalDomainEvent>
{
    public async Task Handle(TestSessionSubmittedForApprovalDomainEvent notification, CancellationToken ct)
    {
        var recipients = await userRepository.GetUsersByPermissionAsync(
            PermissionKeys.TestSessions.WorkflowActions, ct);
        if (recipients.Count == 0) return;

        var payload = JsonSerializer.Serialize(
            new TestSessionSubmittedForApprovalNotificationModel(notification.SessionNo));
        var occurredAt = notification.DateOccurred.ToUnixTimeMilliseconds();

        var inAppNotifications = recipients.Select(recipient => Notification.Create(
            NotificationChannel.InApp,
            TestSessionSubmittedForApprovalNotification.TemplateKey,
            recipient.Id,
            recipient.Email,
            payloadJson: payload,
            language: recipient.GetPreferredLanguage(),
            idempotencyKey:
            $"tssa-{notification.TestSessionId:N}-{recipient.Id:N}-{occurredAt}"
        )).ToList();
        
        await unitOfWork.GetEntityRepository<Notification>().AddRangeAsync(inAppNotifications, ct);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
