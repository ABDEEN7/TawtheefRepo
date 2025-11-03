namespace Tawtheef.Application.Common.Interfaces.Services;

public interface IEmailSender
{
    Task<(bool ok, string? providerId, string? error)> SendAsync(
        string to, string? subject, string body, CancellationToken ct);
}
