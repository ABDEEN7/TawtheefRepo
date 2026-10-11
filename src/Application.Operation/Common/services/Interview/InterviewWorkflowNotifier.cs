using Microsoft.Extensions.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Notifications.Templates.InterviewApprovalDecision;
using Tawtheef.Notifications.Templates.InterviewApprovalRequested;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Application.Operation.Common.Services.Interview;

/// <summary>
/// The submit -> approve / return / cancel notifications shared by templates, committees and schedules.
/// Called from domain event handlers, so it saves itself (see <see cref="IInterviewNotifier.QueueAsync"/>) and
/// never throws: a notification problem must not undo the workflow action.
/// </summary>
public sealed class InterviewWorkflowNotifier(
    IUnitOfWork unitOfWork,
    IInterviewNotifier notifier,
    ICurrentUserService currentUser,
    ILogger<InterviewWorkflowNotifier> logger)
{
    public Guid? CurrentUserId => Guid.TryParse(currentUser.UserId, out var id) ? id : null;

    /// <summary>Tells everyone holding <paramref name="approverPermission"/> (except the submitter) that the item awaits approval.</summary>
    public async Task ApprovalRequestedAsync(
        Func<CancellationToken, Task<InterviewWorkflowItem?>> loadItem, string approverPermission, CancellationToken ct)
    {
        try
        {
            var item = await loadItem(ct);
            if (item is null) return;

            var submitterId = CurrentUserId;
            var approvers = await notifier.GetEmployeesWithPermissionAsync(approverPermission, submitterId, ct);
            if (approvers.Count == 0) return;

            var submitter = await notifier.FindUserAsync(submitterId, ct);

            foreach (var approver in approvers)
            {
                await notifier.QueueAsync(
                    approver,
                    InterviewApprovalRequested.TemplateKey,
                    lang => new InterviewApprovalRequestedModel(
                        item.Kind, item.Name(lang), item.Job(lang), InterviewNotificationText.FullName(submitter, lang), item.LinkPath),
                    lang => InterviewNotificationText.ApprovalRequestedSubject(item.Kind, item.Name(lang), lang),
                    [NotificationChannel.InApp, NotificationChannel.Email],
                    ct);
            }

            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to queue interview approval-request notifications");
        }
    }

    /// <summary>
    /// Tells the item's creator it was approved, returned (with the reason) or cancelled. Nobody is notified
    /// about their own action. A cancellation is informational, so it goes to the bell only.
    /// </summary>
    public async Task DecisionAsync(
        Func<CancellationToken, Task<InterviewWorkflowItem?>> loadItem,
        InterviewDecision decision,
        Guid? creatorId,
        string? reason,
        CancellationToken ct)
    {
        try
        {
            var actorId = CurrentUserId;
            if (creatorId is null || creatorId == actorId) return;

            var creator = await notifier.FindUserAsync(creatorId, ct);
            if (creator is null) return;

            var item = await loadItem(ct);
            if (item is null) return;

            var actor = await notifier.FindUserAsync(actorId, ct);

            NotificationChannel[] channels = decision == InterviewDecision.Cancelled
                ? [NotificationChannel.InApp]
                : [NotificationChannel.InApp, NotificationChannel.Email];

            await notifier.QueueAsync(
                creator,
                InterviewApprovalDecision.TemplateKey,
                lang => new InterviewApprovalDecisionModel(
                    item.Kind, decision, item.Name(lang), item.Job(lang), InterviewNotificationText.FullName(actor, lang),
                    reason, item.LinkPath),
                lang => InterviewNotificationText.DecisionSubject(item.Kind, decision, item.Name(lang), lang),
                channels,
                ct);

            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to queue interview {Decision} notification", decision);
        }
    }
}
