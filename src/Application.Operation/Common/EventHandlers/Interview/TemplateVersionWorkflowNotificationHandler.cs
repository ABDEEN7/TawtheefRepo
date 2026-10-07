using Application.Operation.Common.Services.Interview;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Events.Operation.Employee.Interview.TemplateVersion;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Application.Operation.Common.EventHandlers.Interview;

/// <summary>
/// Evaluation template versions: approvers (interview-evaluation-template.manage) on submit;
/// the version's creator on approve / return / cancel.
/// </summary>
public sealed class TemplateVersionWorkflowNotificationHandler(IUnitOfWork unitOfWork, InterviewWorkflowNotifier workflow)
    : INotificationHandler<TemplateVersionSubmittedEvent>,
        INotificationHandler<TemplateVersionApprovedEvent>,
        INotificationHandler<TemplateVersionReturnedEvent>,
        INotificationHandler<TemplateVersionCancelledEvent>
{
    public Task Handle(TemplateVersionSubmittedEvent notification, CancellationToken ct) =>
        workflow.ApprovalRequestedAsync(Load(notification.Version), PermissionKeys.InterviewEvaluationTemplate.Manage, ct);

    public Task Handle(TemplateVersionApprovedEvent notification, CancellationToken ct) =>
        Decide(notification.Version, InterviewDecision.Approved, ct);

    public Task Handle(TemplateVersionReturnedEvent notification, CancellationToken ct) =>
        Decide(notification.Version, InterviewDecision.Returned, ct);

    public Task Handle(TemplateVersionCancelledEvent notification, CancellationToken ct) =>
        Decide(notification.Version, InterviewDecision.Cancelled, ct);

    private Task Decide(InterviewTemplateVersion version, InterviewDecision decision, CancellationToken ct) =>
        workflow.DecisionAsync(Load(version), decision, version.CreatedById, version.DecisionNotes, ct);

    private Func<CancellationToken, Task<InterviewWorkflowItem?>> Load(InterviewTemplateVersion version) =>
        ct => InterviewWorkflowItems.ForTemplateVersionAsync(unitOfWork, version, ct);
}
