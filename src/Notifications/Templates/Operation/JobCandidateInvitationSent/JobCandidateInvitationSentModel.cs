using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.JobCandidateInvitationSent;

public static class JobCandidateInvitationSent
{
    public const string TemplateKey = nameof(JobCandidateInvitationSent);
}

[NotificationTemplate(JobCandidateInvitationSent.TemplateKey)]
public sealed record JobCandidateInvitationSentModel(string JobTitle);
