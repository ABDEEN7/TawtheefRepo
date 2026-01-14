using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Notification;

public class EmailTemplate : EventEntity
{
    // Logical key: PasswordReset, WelcomeEmail, JobInvitation, etc.
    [MaxLength(100)]
    public required string TemplateKey { get; set; }

    // Email subject line
    [MaxLength(255)]
    public required string Subject { get; set; }

    // HTML/Text template with placeholders {{UserName}}, {{Link}}, etc.
    // Larger than EmailQueue.Body because templates may contain comments & placeholders
    [MaxLength(8000)]
    public required string BodyTemplate { get; set; }

    public bool IsActive { get; set; } = true;
}
