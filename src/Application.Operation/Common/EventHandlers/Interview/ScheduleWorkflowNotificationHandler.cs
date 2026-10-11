using Application.Operation.Common.Services.Interview;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Events.Operation.Employee.Interview.Schedule;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Application.Operation.Common.EventHandlers.Interview;

/// <summary>
/// Interview schedules: approvers (interview-schedule.manage) on submit; the schedule's creator on
/// approve / return / cancel. Committee members are notified separately by
/// <see cref="ScheduleCommitteeNotificationHandler"/> (approve and cancel); candidates are invited manually (Phase 4).
/// </summary>
public sealed class ScheduleWorkflowNotificationHandler(IUnitOfWork unitOfWork, InterviewWorkflowNotifier workflow)
    : INotificationHandler<ScheduleSubmittedEvent>,
        INotificationHandler<ScheduleApprovedEvent>,
        INotificationHandler<ScheduleReturnedEvent>,
        INotificationHandler<ScheduleCancelledEvent>
{
    public Task Handle(ScheduleSubmittedEvent notification, CancellationToken ct) =>
        workflow.ApprovalRequestedAsync(Load(notification.Schedule), PermissionKeys.InterviewSchedule.Manage, ct);

    public Task Handle(ScheduleApprovedEvent notification, CancellationToken ct) =>
        Decide(notification.Schedule, InterviewDecision.Approved, ct);

    public Task Handle(ScheduleReturnedEvent notification, CancellationToken ct) =>
        Decide(notification.Schedule, InterviewDecision.Returned, ct);

    public Task Handle(ScheduleCancelledEvent notification, CancellationToken ct) =>
        Decide(notification.Schedule, InterviewDecision.Cancelled, ct);

    private Task Decide(InterviewSchedule schedule, InterviewDecision decision, CancellationToken ct) =>
        workflow.DecisionAsync(Load(schedule), decision, schedule.CreatedById, schedule.DecisionNotes, ct);

    private Func<CancellationToken, Task<InterviewWorkflowItem?>> Load(InterviewSchedule schedule) =>
        async ct => await InterviewWorkflowItems.ForScheduleAsync(unitOfWork, schedule, ct);
}
