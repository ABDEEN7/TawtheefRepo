using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Domain.Configurations.Settings;

public sealed class QatarResidentOtpSettings
{
    public const string SectionName = "Authentication:QatarResidentOtp";

    [Required]
    public string BaseUrl { get; init; } = string.Empty;
    [Required]
    public required string VerificationPath { get; init; }
    [Required]
    public required string AuthenticationPath { get; init; }
    
    [Required]
    public required string GiveUserName { get; init; }
    [Required]
    public required string Username { get; init; }
    [Required]
    public required string Password { get; init; }
    public int TimeoutSeconds { get; init; } = 30;
}
