using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.JobCandidateInvitationSent;

public static class JobCandidateInvitationSent
{
    public const string TemplateKey = nameof(JobCandidateInvitationSent);
}

[NotificationTemplate(JobCandidateInvitationSent.TemplateKey, "دعوة للتقديم على الوظيفة", "Invitation to Apply for a Job")]
public sealed record JobCandidateInvitationSentModel(string JobTitle, int ExpiryDays, DateOnly ExpiresOn);
