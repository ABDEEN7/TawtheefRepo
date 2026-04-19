namespace Tawtheef.Notifications.Templates.QatarResidentOtp;

public class QatarResidentOtpModel
{
    public const string TemplateKey = "QatarResidentOtp";
    public string Otp { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; }
}
