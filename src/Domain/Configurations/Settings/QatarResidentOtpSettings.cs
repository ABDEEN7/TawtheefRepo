namespace Tawtheef.Domain.Configurations.Settings;

public sealed class QatarResidentOtpSettings
{
    public const string SectionName = "QatarResidentOtp";

    public string BaseUrl { get; init; } = string.Empty;
    public string VerificationPath { get; init; } = "/api/otp/verify-qid";
    public int TimeoutSeconds { get; init; } = 30;
}
