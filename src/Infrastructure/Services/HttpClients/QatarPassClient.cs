using System.Net.Http.Json;
using Application.Recruitment.Common.Interfaces.Services.HttpClients;
using Application.Recruitment.Features.Authenticator.DTOs;
using FluentResults;

namespace Tawtheef.Infrastructure.Services.HttpClients;

public sealed class QatarPassClient(HttpClient http) : IQatarPassClient
{
    public async Task<IResult<QatarPassEnvelope>> GetDataAsync(
        string code,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Fail<QatarPassEnvelope>("QatarPassClient: Code is required.");

        var url = $"api/Services/GetData?Code={Uri.EscapeDataString(code)}";

        HttpResponseMessage res;
        try
        {
            res = await http.GetAsync(url, ct);
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is HttpRequestException)
        {
            return Result.Fail<QatarPassEnvelope>($"QatarPassClient: HTTP error - {ex.Message}");
        }

        if (!res.IsSuccessStatusCode)
            return Result.Fail<QatarPassEnvelope>(
                $"QatarPassClient: Failed to fetch data. Status code: {(int)res.StatusCode}");

        var payload = await res.Content.ReadFromJsonAsync<QatarPassEnvelope>(cancellationToken: ct);

        if (payload is null)
            return Result.Fail<QatarPassEnvelope>("QatarPassClient: Empty payload returned.");

        return Result.Ok(payload);
    }
}

