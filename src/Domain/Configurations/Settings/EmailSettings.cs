using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Domain.Configurations.Settings;

public sealed class EmailSettings
{
    public const string SectionName = "EmailSettings";
    [Required]
    public required string SmtpHost { get; init; }
    public int SmtpPort { get; init; } = 587;
    public string? EmailUser { get; init; }
    public string? EmailPass { get; init; }
    public bool EnableSsl { get; init; } = true;

    public string ProductName { get; init; } = "Tawtheef";
    public string DefaultFromDisplay { get; init; } = "Tawtheef";
    public string? DefaultReplyTo { get; init; }   // e.g. support@yourdomain
    public string? UnsubscribeHttpUrl { get; init; } // e.g. https://.../unsubscribe
    public string? UnsubscribeMailto { get; init; }  // e.g. mailto:unsubscribe@yourdomain
    public bool UseBccForMultiRecipient { get; init; } = true;
    public  string? ManagerEmails { get; init; }
    public string? ContactUsEmail { get; init; }
    public string? LogoPath { get; init; }
    public int MaxSmtpClients { get; init; } = 3;
}
