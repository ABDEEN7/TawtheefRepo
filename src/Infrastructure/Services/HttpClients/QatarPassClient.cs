using System.Net.Http.Json;
using Tawtheef.Application.Features.Authenticator.DTOs;

namespace Tawtheef.Infrastructure.Services.HttpClients;

public class QatarPassClient
{
    private readonly HttpClient _http;

    public QatarPassClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<QatarPassEnvelope?> GetDataAsync(string code, CancellationToken ct)
    {
        var res = await _http.GetAsync($"api/Services/GetData?Code={Uri.EscapeDataString(code)}", ct);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<QatarPassEnvelope>(cancellationToken: ct);
    }
}
