using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Notification;

public class EmailQueue : EventEntity
{
    // RFC-compliant email max length
    [MaxLength(254)]
    public required string RecipientEmail { get; set; }

    // Email subject line (SMTP & UI safe)
    [MaxLength(255)]
    public required string Subject { get; set; }

    // Email body can be large (HTML / text)
    // Still bounded to avoid nvarchar(max) abuse
    [MaxLength(4000)]
    public required string Body { get; set; }

    public bool IsSent { get; set; } = false;
    public DateTimeOffset? SentDate { get; set; }

    public int RetryCount { get; set; } = 0;

    // Store last error / exception message
    [MaxLength(1024)]
    public string? ErrorMessage { get; set; }
}

