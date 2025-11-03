using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Common.Interfaces;

public interface IExternalTokenReader
{
    Task<(string? idToken, string? accessToken)> ReadAsync(HttpContext http, string authenticateScheme, CancellationToken ct = default);
}
