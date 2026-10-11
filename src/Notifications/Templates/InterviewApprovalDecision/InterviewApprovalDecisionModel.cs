using Tawtheef.Notifications.Attributes;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Tawtheef.Notifications.Templates.InterviewApprovalDecision;

public static class InterviewApprovalDecision
{
    public const string TemplateKey = nameof(InterviewApprovalDecision);
}

/// <param name="Reason">Return / cancel reason, or the optional approval note.</param>
/// <param name="LinkPath">Relative operations portal path, e.g. "/portal/interview-templates?template={id}".</param>
[NotificationTemplate(InterviewApprovalDecision.TemplateKey, "تحديث على طلب الاعتماد", "Update on your approval request")]
public sealed record InterviewApprovalDecisionModel(
    InterviewEntityKind Kind,
    InterviewDecision Decision,
    string EntityName,
    string? JobTitle,
    string? DecidedBy,
    string? Reason,
    string LinkPath);
