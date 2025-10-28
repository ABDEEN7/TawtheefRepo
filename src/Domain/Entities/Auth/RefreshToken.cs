using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Auth;

public class RefreshToken : EventEntity
{
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Token { get; init; }
    public DateTime Expires { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? CreatedByIp { get; init; }
    public DateTime? RevokedAt { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? RevokedByIp { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? ReplacedByToken { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? RevokedReason { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;
    
    [MaxLength(length: 128)]
    public string? UserDeviceId { get; init; }
    public Guid UserId { get; init; }
    public User? User { get; init; }

    public void Revoked(DateTime now, string? revokedReason = null)
    {
        RevokedAt = now;
        RevokedReason = revokedReason;
    }
}
