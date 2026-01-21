using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Recruitment.ContactVerificationSent;

public sealed record ContactVerificationSent;

[NotificationTemplate(nameof(ContactVerificationSent))]
public sealed record ContactVerificationSentModel(string Code);
