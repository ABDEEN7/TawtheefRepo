namespace Tawtheef.Application.Common.Interfaces.Services;
public sealed record DeviceInfo(string? Ip, string? UserAgent, string? Platform, string? AppVersion);

public interface ISessionService
{
    Task SetCurrentAsync(string userId, string sessionId, DeviceInfo? device, CancellationToken ct);
    Task<string?> GetCurrentAsync(string userId, CancellationToken ct);
    Task RevokeAllAsync(string userId, CancellationToken ct);
    Task RevokeAsync(string userId, string sessionId, CancellationToken ct);
}
