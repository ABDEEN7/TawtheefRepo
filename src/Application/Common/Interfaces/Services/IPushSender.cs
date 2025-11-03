namespace Tawtheef.Application.Common.Interfaces.Services;

public interface IPushSender
{
    Task<(bool ok, string? providerId, string? error)> SendAsync(
        Guid userId, string title, string body, CancellationToken ct);
}
