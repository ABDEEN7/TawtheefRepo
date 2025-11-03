using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces;

namespace Tawtheef.Infrastructure.Services;

public sealed class CookieExternalTokenReader : IExternalTokenReader
{
    public async Task<(string? idToken, string? accessToken)> ReadAsync(HttpContext http, string authenticateScheme, CancellationToken ct = default)
    {
        var authResult = await http.AuthenticateAsync(authenticateScheme);
        if (!authResult.Succeeded) return (null, null);

        var tokens = authResult.Properties?.GetTokens().ToList();
        var id     = tokens?.FirstOrDefault(t => t.Name == "id_token")?.Value;
        var access = tokens?.FirstOrDefault(t => t.Name == "access_token")?.Value;
        return (id, access);
    }
}
