using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Auth;

public class RefreshToken : EventEntity
{
    [MaxLength(512)]
    public required string TokenHash { get; init; }

    [MaxLength(64)]
    public required string TokenFingerprint { get; init; }

    public DateTime Expires { get; init; }

    [MaxLength(45)]
    public string? CreatedByIp { get; init; }

    public DateTime? RevokedAt { get; set; }

    [MaxLength(45)]
    public string? RevokedByIp { get; set; }

    public Guid? ReplacedByTokenId { get; set; }
    public RefreshToken? ReplacedByToken { get; set; }

    [MaxLength(256)]
    public string? RevokedReason { get; set; }

    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    [MaxLength(128)]
    public string? UserDeviceId { get; init; }

    public Guid UserId { get; init; }
    public User? User { get; init; }

    [MaxLength(64)]
    public required string SecurityStamp { get; set; }

    public void Revoke(DateTime nowUtc, string? revokedByIp = null, string? revokedReason = null)
    {
        RevokedAt = nowUtc;
        RevokedByIp = revokedByIp;
        RevokedReason = revokedReason;
    }
}
