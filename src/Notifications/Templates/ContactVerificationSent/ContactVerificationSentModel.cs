using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ContactVerificationSent;

public static class ContactVerificationSent
{
    public const string TemplateKey = nameof(ContactVerificationSent);
}

[NotificationTemplate(ContactVerificationSent.TemplateKey, "رمز التحقق - منصة التوظيف", "Careers Verification Code")]
public sealed record ContactVerificationSentModel(string Code);
