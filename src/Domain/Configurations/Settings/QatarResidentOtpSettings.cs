namespace Tawtheef.Domain.Configurations.Settings;

public sealed class QatarResidentOtpSettings
{
    public const string SectionName = "Authentication:QatarResidentOtp";

    public string BaseUrl { get; init; } = string.Empty;
    public required string VerificationPath { get; init; }
    public required string AuthenticationPath { get; init; }
    
    public required string GiveUserName { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public int TimeoutSeconds { get; init; } = 30;
}
