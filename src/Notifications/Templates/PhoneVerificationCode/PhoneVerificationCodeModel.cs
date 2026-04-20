namespace Tawtheef.Notifications.Templates.PhoneVerificationCode;

public class PhoneVerificationCodeModel
{
    public const string TemplateKey = "PhoneVerificationCode";
    public string Code { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; }
}
