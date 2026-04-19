using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.User;

namespace Tawtheef.Domain.Entities.Auth;

public enum ContactVerificationType
{
    Phone = 1,
    Email = 2
}

[Index(nameof(UserId))]
public class ContactVerification : EventEntity
{
    public Guid UserId { get; init; }
    public User? User { get; init; }
    
    public ContactVerificationType Type { get; init; }

    /// <summary>Phone number in E.164 format or email address.</summary>
    [MaxLength(256)]
    public required string Destination { get; init; }

    /// <summary>Verification code (OTP).</summary>
    [MaxLength(10)]
    public required string Code { get; init; }

    public DateTime ExpiresAt { get; init; }
    public DateTime? UsedAt { get; set; }

    [MaxLength(10)]
    public string Language { get; set; } = "en";

    public bool IsUsed => UsedAt.HasValue;
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    public void Send(string? language = null)
    {
        if (language != null) Language = language;
        AddDomainEvent(new ContactVerificationSentEvent(UserId, Type, Destination, Code, Language));
    }
}
