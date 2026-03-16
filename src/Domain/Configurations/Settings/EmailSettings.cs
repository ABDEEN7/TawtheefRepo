using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Domain.Configurations.Settings;

public sealed class EmailSettings
{
    public const string SectionName = "EmailSettings";
    [Required]
    public required string SmtpHost { get; init; }
    public int SmtpPort { get; init; } = 587;
    [Required]
    public required string EmailUser { get; init; }
    [Required]
    public required string EmailPass { get; init; }
    public bool EnableSsl { get; init; } = true;

    public string ProductName { get; init; } = "Careers";
    public string DefaultFromDisplay { get; init; } = "Careers";
    public string? DefaultReplyTo { get; init; }   // e.g. support@yourdomain
    public string? UnsubscribeHttpUrl { get; init; } // e.g. https://.../unsubscribe
    public string? UnsubscribeMailto { get; init; }  // e.g. mailto:unsubscribe@yourdomain
    public bool UseBccForMultiRecipient { get; init; } = true;
    [Required]
    public required string ManagerEmails { get; init; }
    [Required]
    public required string ContactUsEmail { get; init; }
    public string? LogoPath { get; init; }
    public string? LogoUrl { get; init; }
    public int MaxSmtpClients { get; init; } = 3;
}
