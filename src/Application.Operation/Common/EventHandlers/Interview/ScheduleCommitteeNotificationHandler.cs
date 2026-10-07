using System.Linq.Expressions;
using Application.Operation.Common.Services.Interview;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.Operation.Employee.Interview.Schedule;
using Tawtheef.Notifications.Templates.InterviewCommitteeAssignment;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Application.Operation.Common.EventHandlers.Interview;

/// <summary>
/// Tells the active members of every committee that has live interviews in a schedule when that schedule is
/// approved, and again if it is cancelled afterwards: their role, how many interviews their committee has and
/// the date range (Qatar time). Members only hear about approved schedules, so cancelling a Draft / Proposed
/// schedule notifies nobody.
/// </summary>
public sealed class ScheduleCommitteeNotificationHandler(
    IUnitOfWork unitOfWork,
    IInterviewNotifier notifier,
    InterviewWorkflowNotifier workflow,
    ILogger<ScheduleCommitteeNotificationHandler> logger)
    : INotificationHandler<ScheduleApprovedEvent>,
        INotificationHandler<ScheduleCancelledEvent>
{
    public Task Handle(ScheduleApprovedEvent notification, CancellationToken ct) =>
        NotifyAsync(notification.Schedule, InterviewAssignmentChange.ScheduleApproved, InterviewAppointment.IsLiveCandidate, ct);

    // ApprovedAt is only set by Approve and an Approved schedule can't go back to Draft, so a cancelled
    // schedule with ApprovedAt was Approved when it was cancelled - i.e. its members had been told.
    // The interviews that won't happen are the bookings cancelled with it. They are matched by id, not by
    // status: other handlers of this same save may already have written them as Cancelled.
    public Task Handle(ScheduleCancelledEvent notification, CancellationToken ct)
    {
        if (notification.Schedule.ApprovedAt is null) return Task.CompletedTask;

        var cancelledIds = notification.CancelledAppointmentIds.ToList();
        return NotifyAsync(notification.Schedule, InterviewAssignmentChange.ScheduleCancelled, a => cancelledIds.Contains(a.Id), ct);
    }

    private async Task NotifyAsync(
        InterviewSchedule schedule,
        InterviewAssignmentChange change,
        Expression<Func<InterviewAppointment, bool>> interviews,
        CancellationToken ct)
    {
        try
        {
            var perCommittee = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
                .AsNoTracking()
                .Where(a => a.InterviewScheduleId == schedule.Id)
                .Where(interviews)
                .GroupBy(a => a.InterviewCommitteeId)
                .Select(g => new { CommitteeId = g.Key, Count = g.Count(), From = g.Min(a => a.StartAt), To = g.Max(a => a.StartAt) })
                .ToListAsync(ct);
            if (perCommittee.Count == 0) return;

            var committeeIds = perCommittee.Select(c => c.CommitteeId).ToList();
            var committees = await unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet
                .AsNoTracking()
                .Where(c => committeeIds.Contains(c.Id))
                .Select(c => new
                {
                    c.Id,
                    c.NameAr,
                    c.NameEn,
                    c.Code,
                    Members = c.Members.Where(m => m.IsActive).Select(m => new { m.MemberUserId, m.Role }).ToList()
                })
                .ToDictionaryAsync(c => c.Id, ct);

            var (jobAr, jobEn) = await InterviewWorkflowItems.JobTitleAsync(unitOfWork, schedule.JobId, ct);
            var actorId = workflow.CurrentUserId;
            // Both link to the schedule itself: for a cancelled one it opens marked Cancelled with the affected
            // candidates (the plain session list hides cancelled schedules, so it would show nothing).
            var linkPath = $"{InterviewNotificationLinks.StartInterviewPath}?schedule={schedule.Id}";
            var reason = change == InterviewAssignmentChange.ScheduleCancelled ? schedule.DecisionNotes : null;
            var queued = false;

            foreach (var stats in perCommittee)
            {
                if (!committees.TryGetValue(stats.CommitteeId, out var committee)) continue;

                foreach (var member in committee.Members.Where(m => m.MemberUserId != actorId))
                {
                    var user = await notifier.FindUserAsync(member.MemberUserId, ct);
                    if (user is null || user.IsBlocked || user.IsDeleted) continue;

                    await notifier.QueueAsync(
                        user,
                        InterviewCommitteeAssignment.TemplateKey,
                        lang =>
                        {
                            string? job = jobAr is null ? null : InterviewNotificationText.Pick(lang, jobAr, jobEn);
                            return new InterviewCommitteeAssignmentModel(
                                change,
                                CommitteeName(lang),
                                job,
                                InterviewNotificationText.RoleName(member.Role, lang),
                                InterviewNotificationText.Pick(lang, schedule.TitleAr, schedule.TitleEn),
                                InterviewNotificationText.QatarDate(stats.From),
                                InterviewNotificationText.QatarDate(stats.To),
                                stats.Count,
                                linkPath,
                                reason);
                        },
                        lang => InterviewNotificationText.AssignmentSubject(change, CommitteeName(lang), lang),
                        [NotificationChannel.InApp, NotificationChannel.Email],
                        ct);
                    queued = true;
                }

                string CommitteeName(string lang) => InterviewWorkflowItems.WithCode(
                    InterviewNotificationText.Pick(lang, committee.NameAr, committee.NameEn), committee.Code);
            }

            if (queued) await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // A notification failure must not undo the schedule approval / cancellation itself.
            logger.LogError(ex, "Failed to queue {Change} committee notifications for schedule {ScheduleId}", change, schedule.Id);
        }
    }
}
