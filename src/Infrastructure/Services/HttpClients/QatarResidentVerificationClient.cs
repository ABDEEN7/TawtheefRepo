using System.Net.Http.Json;
using FluentResults;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Features.Authenticator.DTOs;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Infrastructure.Services.HttpClients;

public sealed class QatarResidentVerificationClient(
    HttpClient httpClient,
    IOptions<QatarResidentOtpSettings> settings)
    : IQatarResidentVerificationClient
{
    private readonly QatarResidentOtpSettings _settings = settings.Value;

    public async Task<IResult<QatarResidentVerificationResult>> VerifyAsync(string qid, string phoneNumber, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(qid) || string.IsNullOrWhiteSpace(phoneNumber))
            return Result.Fail<QatarResidentVerificationResult>(ErrorsCodes.QatarResidentVerificationFailed);

        HttpResponseMessage response;
        try
        {
            response = await httpClient.PostAsJsonAsync(_settings.VerificationPath, new { qid, phoneNumber }, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return Result.Fail<QatarResidentVerificationResult>($"QatarResidentVerificationClient: HTTP error - {ex.Message}");
        }

        if (!response.IsSuccessStatusCode)
            return Result.Fail<QatarResidentVerificationResult>(
                $"QatarResidentVerificationClient: Failed to verify. Status code: {(int)response.StatusCode}");

        var payload = await response.Content.ReadFromJsonAsync<QatarResidentVerificationResult>(cancellationToken: ct);
        if (payload is null)
            return Result.Fail<QatarResidentVerificationResult>(ErrorsCodes.QatarResidentVerificationFailed);

        return Result.Ok(payload);
    }
}
