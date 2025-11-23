using FluentResults;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface ISmsSender
{
    Task<(bool ok, string? providerId, IReadOnlyList<IError>? error)> SendAsync(string phoneE164, string body, CancellationToken ct);
}
