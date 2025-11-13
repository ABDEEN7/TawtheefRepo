namespace Tawtheef.Application.Common.Interfaces.Services;
public sealed record DeviceInfo(string? Ip, string? UserAgent, string? Platform, string? AppVersion);

public interface ISessionService
{
    Task SetCurrentAsync(Guid userId, string sessionId, DeviceInfo? device, CancellationToken ct);
    Task<string?> GetCurrentAsync(Guid userId, CancellationToken ct);
    Task RevokeAllAsync(Guid userId, CancellationToken ct);
    Task RevokeAsync(Guid userId, string sessionId, CancellationToken ct);
}
