using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.JobCandidateInvitationSent;

public static class JobCandidateInvitationSent
{
    public const string TemplateKey = nameof(JobCandidateInvitationSent);
}

[NotificationTemplate(JobCandidateInvitationSent.TemplateKey, "دعوة لتقديم طلب للوظيفة", "Careers Job Invitation")]
public sealed record JobCandidateInvitationSentModel(string JobTitle);
