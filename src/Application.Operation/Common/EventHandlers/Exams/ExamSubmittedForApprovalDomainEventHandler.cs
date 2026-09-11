using System.Text.Json;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.Operation.Employee.Exams;
using Tawtheef.Notifications.Templates.ExamSubmittedForApprovalNotification;

namespace Application.Operation.Common.EventHandlers.Exams;

public sealed class ExamSubmittedForApprovalDomainEventHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : INotificationHandler<ExamSubmittedForApprovalDomainEvent>
{
    public async Task Handle(ExamSubmittedForApprovalDomainEvent notification, CancellationToken ct)
    {
        var reviewers = await userRepository.GetUsersByPermissionAsync(
            PermissionKeys.Exams.WorkflowActions, ct);
        var payload = JsonSerializer.Serialize(new ExamSubmittedForApprovalNotificationModel(
            notification.ExamNumber,
            notification.ExamTitleAr,
            notification.ExamTitleEn,
            notification.JobTitleAr,
            notification.JobTitleEn));

        foreach (var reviewer in reviewers)
        {
            var inAppNotification = Notification.Create(
                NotificationChannel.InApp,
                ExamSubmittedForApprovalNotification.TemplateKey,
                reviewer.Id,
                reviewer.Email,
                payloadJson: payload,
                language: reviewer.GetPreferredLanguage(),
                idempotencyKey: $"exam-submitted-{notification.ExamId:N}-" +
                                $"{notification.DateOccurred.UtcDateTime.Ticks}-{reviewer.Id:N}");
            await unitOfWork.GetEntityRepository<Notification>().AddAsync(inAppNotification, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}
