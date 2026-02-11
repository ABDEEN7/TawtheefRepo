using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Auth;

public class RefreshToken : EventEntity
{
    // Refresh tokens are usually long, random, URL-safe strings
    // Common range: 256–512 chars
    [MaxLength(512)]
    public required string Token { get; init; }

    public DateTime Expires { get; init; }

    // IPv4 max = 15 chars, IPv6 max = 45 chars
    [MaxLength(45)]
    public string? CreatedByIp { get; init; }

    public DateTime? RevokedAt { get; set; }

    // IPv4 / IPv6
    [MaxLength(45)]
    public string? RevokedByIp { get; set; }

    // Same format/length as Token
    [MaxLength(512)]
    public string? ReplacedByToken { get; set; }

    // Short audit / security reason text
    [MaxLength(256)]
    public string? RevokedReason { get; set; }

    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    // Device fingerprint / client-generated ID
    [MaxLength(128)]
    public string? UserDeviceId { get; init; }

    public Guid UserId { get; init; }
    public User? User { get; init; }

    // ASP.NET Identity security stamp is 32–64 chars (GUID/string)
    [MaxLength(64)]
    public required string SecurityStamp { get; set; }

    public void Revoked(DateTime now, string? revokedReason = null)
    {
        RevokedAt = now;
        RevokedReason = revokedReason;
    }
}

