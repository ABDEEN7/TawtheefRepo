using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.Recruitment.ContactVerificationSent;

public static class ContactVerificationSent
{
    public const string TemplateKey = nameof(ContactVerificationSent);
}

[NotificationTemplate(ContactVerificationSent.TemplateKey)]
public sealed record ContactVerificationSentModel(string Code);
