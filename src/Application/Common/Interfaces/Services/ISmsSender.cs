namespace Tawtheef.Application.Common.Interfaces.Services;

public interface ISmsSender
{
    Task<(bool ok, string? providerId, string? error)> SendAsync(string phoneE164, string body, CancellationToken ct);
}
