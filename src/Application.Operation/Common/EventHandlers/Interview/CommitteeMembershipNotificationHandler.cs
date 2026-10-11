using Application.Operation.Common.Services.Interview;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.Operation.Employee.Interview.Committee;
using Tawtheef.Notifications.Templates.InterviewCommitteeAssignment;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Application.Operation.Common.EventHandlers.Interview;

/// <summary>
/// Tells committee members about their membership: every active member when the committee is approved,
/// and members added to / removed from it afterwards. Roster edits while the committee is still a draft
/// (they raise the same member events) notify nobody.
/// </summary>
public sealed class CommitteeMembershipNotificationHandler(
    IUnitOfWork unitOfWork,
    IInterviewNotifier notifier,
    InterviewWorkflowNotifier workflow,
    ILogger<CommitteeMembershipNotificationHandler> logger)
    : INotificationHandler<CommitteeApprovedEvent>,
        INotificationHandler<CommitteeMemberAddedEvent>,
        INotificationHandler<CommitteeMemberRemovedEvent>
{
    public Task Handle(CommitteeApprovedEvent notification, CancellationToken ct)
    {
        var committeeId = notification.Committee.Id;
        return NotifyAsync(notification.Committee, async c => await unitOfWork.GetEntityRepository<InterviewCommitteeMember>().DbSet
                .AsNoTracking()
                .Where(m => m.InterviewCommitteeId == committeeId && m.IsActive)
                .Select(m => new Member(m.MemberUserId, m.Role))
                .ToListAsync(c),
            InterviewAssignmentChange.Assigned, ct);
    }

    public Task Handle(CommitteeMemberAddedEvent notification, CancellationToken ct) =>
        notification.Committee.Status == CommitteeStatus.Approved
            ? NotifyAsync(notification.Committee, Single(notification.Member), InterviewAssignmentChange.Assigned, ct)
            : Task.CompletedTask;

    public Task Handle(CommitteeMemberRemovedEvent notification, CancellationToken ct) =>
        notification.Committee.Status == CommitteeStatus.Approved
            ? NotifyAsync(notification.Committee, Single(notification.Member), InterviewAssignmentChange.Removed, ct)
            : Task.CompletedTask;

    private static Func<CancellationToken, Task<List<Member>>> Single(InterviewCommitteeMember member) =>
        _ => Task.FromResult(new List<Member> { new(member.MemberUserId, member.Role) });

    private async Task NotifyAsync(
        InterviewCommittee committee,
        Func<CancellationToken, Task<List<Member>>> loadMembers,
        InterviewAssignmentChange change,
        CancellationToken ct)
    {
        try
        {
            var actorId = workflow.CurrentUserId;
            var recipients = (await loadMembers(ct)).Where(m => m.UserId != actorId).ToList();
            if (recipients.Count == 0) return;

            var item = await InterviewWorkflowItems.ForCommitteeAsync(unitOfWork, committee, ct);
            // A removed member may no longer see the committee, so send them to the list instead of its detail.
            var linkPath = change == InterviewAssignmentChange.Removed
                ? InterviewCommitteeAssignment.DefaultPath(change)
                : item.LinkPath;
            // Removal is informational; a new assignment needs the member's attention, so it is emailed too.
            NotificationChannel[] channels = change == InterviewAssignmentChange.Removed
                ? [NotificationChannel.InApp]
                : [NotificationChannel.InApp, NotificationChannel.Email];

            foreach (var member in recipients)
            {
                var user = await notifier.FindUserAsync(member.UserId, ct);
                if (user is null || user.IsBlocked || user.IsDeleted) continue;

                await notifier.QueueAsync(
                    user,
                    InterviewCommitteeAssignment.TemplateKey,
                    lang => new InterviewCommitteeAssignmentModel(
                        change,
                        item.Name(lang),
                        item.Job(lang),
                        change == InterviewAssignmentChange.Assigned ? InterviewNotificationText.RoleName(member.Role, lang) : null,
                        null, null, null, null,
                        linkPath),
                    lang => InterviewNotificationText.AssignmentSubject(change, item.Name(lang), lang),
                    channels,
                    ct);
            }

            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // A notification failure must not undo the committee change itself.
            logger.LogError(ex, "Failed to queue {Change} notifications for committee {CommitteeId}", change, committee.Id);
        }
    }

    private sealed record Member(Guid UserId, CommitteeRole Role);
}
