using Application.Operation.Common.Services.Interview;
using MediatR;
using Microsoft.Extensions.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.Operation.Employee.Interview.TemplateVersion;
using Tawtheef.Notifications.Templates.InterviewApprovalRequested;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Application.Operation.Common.EventHandlers.Interview;

/// <summary>Tells every template approver (interview-evaluation-template.manage, except the submitter) that a version awaits approval.</summary>
public sealed class TemplateVersionSubmittedNotificationHandler(
    IUnitOfWork unitOfWork,
    IInterviewNotifier notifier,
    ICurrentUserService currentUser,
    ILogger<TemplateVersionSubmittedNotificationHandler> logger)
    : INotificationHandler<TemplateVersionSubmittedEvent>
{
    public async Task Handle(TemplateVersionSubmittedEvent notification, CancellationToken ct)
    {
        try
        {
            var info = await TemplateVersionNotificationInfo.LoadAsync(unitOfWork, notification.Version, ct);
            if (info is null) return;

            Guid? submitterId = Guid.TryParse(currentUser.UserId, out var id) ? id : null;
            var approvers = await notifier.GetEmployeesWithPermissionAsync(
                PermissionKeys.InterviewEvaluationTemplate.Manage, submitterId, ct);
            if (approvers.Count == 0) return;

            var submitter = await notifier.FindUserAsync(submitterId, ct);

            foreach (var approver in approvers)
            {
                await notifier.QueueAsync(
                    approver,
                    InterviewApprovalRequested.TemplateKey,
                    lang => new InterviewApprovalRequestedModel(
                        InterviewEntityKind.Template,
                        info.Name(lang),
                        info.Job(lang),
                        InterviewNotificationText.FullName(submitter, lang),
                        info.LinkPath),
                    lang => InterviewNotificationText.ApprovalRequestedSubject(InterviewEntityKind.Template, info.Name(lang), lang),
                    [NotificationChannel.InApp, NotificationChannel.Email],
                    ct);
            }

            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // A notification failure must not undo the submit itself.
            logger.LogError(ex, "Failed to queue approval-request notifications for template version {VersionId}",
                notification.Version.Id);
        }
    }
}
