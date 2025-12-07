namespace Tawtheef.Application.Common.Interfaces.Services;

public sealed record LoginAttemptEntry(
    Guid? UserId,
    Guid? UserTypeId,
    string Source,
    bool Succeeded,
    string? FailureReason = null,
    string? SessionId = null,
    string? IpAddress = null,
    DateTimeOffset? AttemptedAt = null);

public interface ILoginAuditService
{
    Task LogAsync(LoginAttemptEntry entry, CancellationToken ct);
}
