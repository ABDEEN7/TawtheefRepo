using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Auth;

public class OTPRequest: EventEntity
{
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
    
    [MaxLength(6)]
    public required string Otp { get; set; }
    public DateTimeOffset ExpiryDate { get; set; }
    public bool IsUsed { get; set; } = false;
    public bool IsActive { get; set; } = true;
}
