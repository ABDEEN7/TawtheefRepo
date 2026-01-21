using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Operation.JobCandidateInvitationSent;

public sealed record JobCandidateInvitationSent;

[NotificationTemplate(nameof(JobCandidateInvitationSent))]
public sealed record JobCandidateInvitationSentModel(string JobTitle, string OrganizationName, string Notes, string PortalUrl);
