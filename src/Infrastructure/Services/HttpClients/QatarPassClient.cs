using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Features.Authenticator.DTOs;
using Tawtheef.Domain.Configurations;

namespace Tawtheef.Infrastructure.Services.HttpClients;

public class QatarPassClient : IQatarPassClient
{
    private readonly HttpClient _http;
    private readonly QatarPassAuthSettings _opt;
    public QatarPassClient(HttpClient http, IOptions<QatarPassAuthSettings> opt)
    {
        _http = http;
        _opt  = opt.Value;
        _http.BaseAddress ??= new Uri(_opt.BaseUrl);
        _http.Timeout = TimeSpan.FromSeconds(10);
    }
    public async Task<Result<QatarPassEnvelope>> GetDataAsync(string code, CancellationToken ct)
    {
        var res = await _http.GetAsync($"api/Services/GetData?Code={Uri.EscapeDataString(code)}", ct);
        if (!res.IsSuccessStatusCode)
            return Result.Failure<QatarPassEnvelope>($"QatarPassClient: Failed to fetch data. Status code: {(int)res.StatusCode}");
        var payload = await res.Content.ReadFromJsonAsync<QatarPassEnvelope>(cancellationToken: ct);
        if(payload is null)
            return Result.Failure<QatarPassEnvelope>("QatarPassClient: Empty payload returned.");
        
        return Result.Success(payload);
    }
}


