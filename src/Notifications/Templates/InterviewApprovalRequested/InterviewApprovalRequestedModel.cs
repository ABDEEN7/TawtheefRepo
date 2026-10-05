using Tawtheef.Notifications.Attributes;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Tawtheef.Notifications.Templates.InterviewApprovalRequested;

public static class InterviewApprovalRequested
{
    public const string TemplateKey = nameof(InterviewApprovalRequested);
}

/// <param name="EntityName">Template title / committee name / schedule title / report code.</param>
/// <param name="LinkPath">Relative operations portal path, e.g. "/portal/interview-templates?template={id}".</param>
[NotificationTemplate(InterviewApprovalRequested.TemplateKey, "طلب اعتماد جديد", "New approval request")]
public sealed record InterviewApprovalRequestedModel(
    InterviewEntityKind Kind,
    string EntityName,
    string? JobTitle,
    string? SubmittedBy,
    string LinkPath);
