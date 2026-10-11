using Application.Operation.Common.Services.Interview;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Events.Operation.Employee.Interview.Committee;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Application.Operation.Common.EventHandlers.Interview;

/// <summary>
/// Interview committees: approvers (interview-committee.manage) on submit; the committee's creator on
/// approve / return / cancel. Members are notified separately by <see cref="CommitteeMembershipNotificationHandler"/>.
/// </summary>
public sealed class CommitteeWorkflowNotificationHandler(IUnitOfWork unitOfWork, InterviewWorkflowNotifier workflow)
    : INotificationHandler<CommitteeSubmittedEvent>,
        INotificationHandler<CommitteeApprovedEvent>,
        INotificationHandler<CommitteeReturnedEvent>,
        INotificationHandler<CommitteeCancelledEvent>
{
    public Task Handle(CommitteeSubmittedEvent notification, CancellationToken ct) =>
        workflow.ApprovalRequestedAsync(Load(notification.Committee), PermissionKeys.InterviewCommittee.Manage, ct);

    public Task Handle(CommitteeApprovedEvent notification, CancellationToken ct) =>
        Decide(notification.Committee, InterviewDecision.Approved, ct);

    public Task Handle(CommitteeReturnedEvent notification, CancellationToken ct) =>
        Decide(notification.Committee, InterviewDecision.Returned, ct);

    public Task Handle(CommitteeCancelledEvent notification, CancellationToken ct) =>
        Decide(notification.Committee, InterviewDecision.Cancelled, ct);

    private Task Decide(InterviewCommittee committee, InterviewDecision decision, CancellationToken ct) =>
        workflow.DecisionAsync(Load(committee), decision, committee.CreatedById, committee.DecisionNotes, ct);

    private Func<CancellationToken, Task<InterviewWorkflowItem?>> Load(InterviewCommittee committee) =>
        async ct => await InterviewWorkflowItems.ForCommitteeAsync(unitOfWork, committee, ct);
}
