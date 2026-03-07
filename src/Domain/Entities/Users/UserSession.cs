using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Users;

[Index(nameof(SessionId))]
[Index(nameof(UserId), nameof(SessionId))]
public sealed class UserSession : EventEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public required string SessionId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAtUtc { get; set; }

    public string? Ip { get; set; }
    public string? UserAgent { get; set; }
    public string? Platform { get; set; }
    public string? AppVersion { get; set; }
}
