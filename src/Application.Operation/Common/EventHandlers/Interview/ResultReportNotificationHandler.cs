using Application.Operation.Common.Services.Interview;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.Operation.Employee.Interview.ResultReport;
using Tawtheef.Notifications.Templates.InterviewResultReview;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Application.Operation.Common.EventHandlers.Interview;

/// <summary>
/// Interview result reports: the committee chair when a new report is ready for their review; the final
/// approvers (interview-result-report.manage) when the chair sends it on; the chair who sent it and the
/// schedule's creator when it is approved.
/// </summary>
public sealed class ResultReportNotificationHandler(
    IUnitOfWork unitOfWork,
    IInterviewNotifier notifier,
    InterviewWorkflowNotifier workflow,
    ILogger<ResultReportNotificationHandler> logger)
    : INotificationHandler<ResultReportReadyForCommitteeReviewEvent>,
        INotificationHandler<ResultReportSentForApprovalEvent>,
        INotificationHandler<ResultReportApprovedEvent>
{
    // System-generated (when the schedule's last candidate is finished), so there is no actor to skip:
    // the chair is told even when their own last evaluation is what completed the schedule.
    public async Task Handle(ResultReportReadyForCommitteeReviewEvent notification, CancellationToken ct)
    {
        var report = notification.Report;
        try
        {
            var schedule = await unitOfWork.GetEntityRepository<InterviewSchedule>().DbSet
                .AsNoTracking()
                .Where(s => s.Id == report.InterviewScheduleId)
                .Select(s => new
                {
                    s.TitleAr,
                    s.TitleEn,
                    s.JobId,
                    // All appointments of a schedule belong to the same committee.
                    CommitteeId = s.Appointments.Select(a => (Guid?)a.InterviewCommitteeId).FirstOrDefault()
                })
                .FirstOrDefaultAsync(ct);
            if (schedule?.CommitteeId is null) return;

            var committee = await unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet
                .AsNoTracking()
                .Where(c => c.Id == schedule.CommitteeId)
                .Select(c => new
                {
                    c.NameAr,
                    c.NameEn,
                    c.Code,
                    ChairId = c.Members
                        .Where(m => m.IsActive && m.Role == CommitteeRole.Chair)
                        .Select(m => (Guid?)m.MemberUserId)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(ct);
            if (committee?.ChairId is null) return;

            var chair = await notifier.FindUserAsync(committee.ChairId, ct);
            if (chair is null || chair.IsBlocked || chair.IsDeleted) return;

            var (jobAr, jobEn) = await InterviewWorkflowItems.JobTitleAsync(unitOfWork, schedule.JobId, ct);
            var linkPath = $"{InterviewNotificationLinks.StartInterviewPath}?schedule={report.InterviewScheduleId}&review=1";

            await notifier.QueueAsync(
                chair,
                InterviewResultReview.TemplateKey,
                lang => new InterviewResultReviewModel(
                    InterviewNotificationText.Pick(lang, schedule.TitleAr, schedule.TitleEn),
                    jobAr is null ? null : InterviewNotificationText.Pick(lang, jobAr, jobEn),
                    InterviewWorkflowItems.WithCode(InterviewNotificationText.Pick(lang, committee.NameAr, committee.NameEn), committee.Code),
                    report.Candidates.Count,
                    linkPath),
                lang => InterviewNotificationText.ResultReviewSubject(
                    InterviewNotificationText.Pick(lang, schedule.TitleAr, schedule.TitleEn), lang),
                [NotificationChannel.InApp, NotificationChannel.Email],
                ct);

            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // A notification failure must not undo the report generation itself.
            logger.LogError(ex, "Failed to queue the committee-review notification for schedule {ScheduleId}",
                report.InterviewScheduleId);
        }
    }

    public Task Handle(ResultReportSentForApprovalEvent notification, CancellationToken ct) =>
        workflow.ApprovalRequestedAsync(Load(notification.Report), PermissionKeys.InterviewResultReport.Manage, ct);

    // The chair who sent it and whoever created the schedule; DecisionAsync skips the approver themselves.
    public async Task Handle(ResultReportApprovedEvent notification, CancellationToken ct)
    {
        var report = notification.Report;
        Guid? scheduleCreatorId;
        try
        {
            scheduleCreatorId = await unitOfWork.GetEntityRepository<InterviewSchedule>().DbSet
                .AsNoTracking()
                .Where(s => s.Id == report.InterviewScheduleId)
                .Select(s => s.CreatedById)
                .FirstOrDefaultAsync(ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to load the schedule creator for approved result report of schedule {ScheduleId}",
                report.InterviewScheduleId);
            return;
        }

        foreach (var recipientId in new[] { report.CommitteeReviewedById, scheduleCreatorId }.Where(id => id is not null).Distinct())
            await workflow.DecisionAsync(Load(report), InterviewDecision.Approved, recipientId, null, ct);
    }

    private Func<CancellationToken, Task<InterviewWorkflowItem?>> Load(InterviewResultReport report) =>
        ct => InterviewWorkflowItems.ForResultReportAsync(unitOfWork, report, ct);
}
