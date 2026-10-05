using Application.Operation.Common.Services.Interview;
using MediatR;
using Microsoft.Extensions.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.Operation.Employee.Interview.TemplateVersion;
using Tawtheef.Notifications.Templates.InterviewApprovalDecision;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Application.Operation.Common.EventHandlers.Interview;

/// <summary>
/// Tells the version's creator that it was approved, returned (with the reason) or cancelled.
/// Nobody is notified about their own action, e.g. a creator cancelling their own draft.
/// </summary>
public sealed class TemplateVersionDecisionNotificationHandler(
    IUnitOfWork unitOfWork,
    IInterviewNotifier notifier,
    ICurrentUserService currentUser,
    ILogger<TemplateVersionDecisionNotificationHandler> logger)
    : INotificationHandler<TemplateVersionApprovedEvent>,
        INotificationHandler<TemplateVersionReturnedEvent>,
        INotificationHandler<TemplateVersionCancelledEvent>
{
    public Task Handle(TemplateVersionApprovedEvent notification, CancellationToken ct) =>
        NotifyAsync(notification.Version, InterviewDecision.Approved, ct);

    public Task Handle(TemplateVersionReturnedEvent notification, CancellationToken ct) =>
        NotifyAsync(notification.Version, InterviewDecision.Returned, ct);

    public Task Handle(TemplateVersionCancelledEvent notification, CancellationToken ct) =>
        NotifyAsync(notification.Version, InterviewDecision.Cancelled, ct);

    private async Task NotifyAsync(InterviewTemplateVersion version, InterviewDecision decision, CancellationToken ct)
    {
        try
        {
            Guid? actorId = Guid.TryParse(currentUser.UserId, out var id) ? id : null;
            if (version.CreatedById is null || version.CreatedById == actorId) return;

            var creator = await notifier.FindUserAsync(version.CreatedById, ct);
            if (creator is null) return;

            var info = await TemplateVersionNotificationInfo.LoadAsync(unitOfWork, version, ct);
            if (info is null) return;

            var actor = await notifier.FindUserAsync(actorId, ct);

            // A cancellation is informational only; approve and return need the creator to act, so they are emailed too.
            NotificationChannel[] channels = decision == InterviewDecision.Cancelled
                ? [NotificationChannel.InApp]
                : [NotificationChannel.InApp, NotificationChannel.Email];

            await notifier.QueueAsync(
                creator,
                InterviewApprovalDecision.TemplateKey,
                lang => new InterviewApprovalDecisionModel(
                    InterviewEntityKind.Template,
                    decision,
                    info.Name(lang),
                    info.Job(lang),
                    InterviewNotificationText.FullName(actor, lang),
                    version.DecisionNotes,
                    info.LinkPath),
                lang => InterviewNotificationText.DecisionSubject(InterviewEntityKind.Template, decision, info.Name(lang), lang),
                channels,
                ct);

            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // A notification failure must not undo the approval decision itself.
            logger.LogError(ex, "Failed to queue {Decision} notification for template version {VersionId}",
                decision, version.Id);
        }
    }
}
