using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Auth;

public class RefreshToken : EventEntity
{
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Token { get; init; }
    public DateTimeOffset Expires { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? CreatedByIp { get; init; }
    public DateTimeOffset? RevokedAt { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? RevokedByIp { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? ReplacedByToken { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? RevokedReason { get; set; }
    
    public bool IsExpired => DateTimeOffset.UtcNow >= Expires;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;
    
    [MaxLength(length: 128)]
    public string? UserDeviceId { get; init; }
    public Guid UserId { get; init; }
    public User? User { get; init; }
    public required string SecurityStamp { get; set; }

    public void Revoked(DateTimeOffset now, string? revokedReason = null)
    {
        RevokedAt = now;
        RevokedReason = revokedReason;
    }
}
