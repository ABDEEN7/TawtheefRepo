using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Auth;

public sealed class LoginAttempt : EventEntity
{
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public Guid? UserTypeId { get; set; }
    public UserType? UserType { get; set; }

    // Web / Mobile / API / External / SSO provider name
    [MaxLength(50)]
    public required string Source { get; set; }

    public bool Succeeded { get; set; }

    // Short, controlled failure messages (e.g. "InvalidPassword", "UserLocked")
    [MaxLength(256)]
    public string? FailureReason { get; set; }

    // Session identifiers / correlation IDs
    [MaxLength(128)]
    public string? SessionId { get; set; }

    public DateTime AttemptedAtUtc { get; set; } = DateTime.UtcNow;

    // IPv4 / IPv6 support
    [MaxLength(45)]
    public string? IpAddress { get; set; }
}

