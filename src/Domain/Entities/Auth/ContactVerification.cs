using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.User;

namespace Tawtheef.Domain.Entities.Auth;

public enum ContactVerificationType
{
    Phone = 1,
    Email = 2
}

public class ContactVerification : EventEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    public ContactVerificationType Type { get; set; }

    /// <summary>Phone number in E.164 format or email address.</summary>
    [MaxLength(256)]
    public required string Destination { get; set; }

    /// <summary>Verification code (OTP).</summary>
    [MaxLength(10)]
    public required string Code { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? UsedAt { get; set; }

    public bool IsUsed => UsedAt.HasValue;
    public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;

    public void Send()
    {
        AddDomainEvent(new ContactVerificationSentEvent(UserId, Type, Destination, Code));
    }
}
