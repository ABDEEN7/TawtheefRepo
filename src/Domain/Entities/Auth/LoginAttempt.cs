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

    public required string Source { get; set; }
    public bool Succeeded { get; set; }
    public string? FailureReason { get; set; }
    public string? SessionId { get; set; }
    public DateTime AttemptedAtUtc { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }
}
