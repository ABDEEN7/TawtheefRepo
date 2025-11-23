using FluentResults;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface IPushSender
{
    Task<(bool ok, string? providerId, IReadOnlyList<IError>? error)> SendAsync(
        Guid userId, string title, string body, CancellationToken ct);
}
