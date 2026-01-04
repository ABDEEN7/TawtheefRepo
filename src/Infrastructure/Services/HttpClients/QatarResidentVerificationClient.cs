using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentResults;
using Microsoft.Extensions.Options;
using Serilog;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Features.Authenticator.DTOs;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Infrastructure.Services.HttpClients;

public sealed class QatarResidentVerificationClient(HttpClient httpClient, ILogger logger,
    IOptions<QatarResidentOtpSettings> settings) : IQatarResidentVerificationClient
{
    private readonly QatarResidentOtpSettings _settings = settings.Value;

    public async Task<IResult<bool>> VerifyAsync(string qid, string phoneNumber, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(qid) || string.IsNullOrWhiteSpace(phoneNumber))
            return Result.Fail<bool>(ErrorsCodes.QatarResidentVerificationFailed);

        var authResult = await AuthenticateAsync(ct);
        if (authResult.IsFailed) return Result.Fail<bool>(authResult.Errors);
        if (authResult.Value is null) return Result.Fail<bool>(ErrorsCodes.QatarResidentVerificationFailed);
        if (string.IsNullOrWhiteSpace(authResult.Value.AccessToken)) return Result.Fail<bool>(ErrorsCodes.QatarResidentVerificationFailed);
        
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.Value.AccessToken);
        var response = await httpClient.GetAsync(BuildCustomerValidationUri(qid, phoneNumber), ct);
        if (!response.IsSuccessStatusCode)
            return Result.Fail<bool>(
                $"QatarResidentVerificationClient: Failed to verify. Status code: {(int)response.StatusCode}");

        var payload = await response.Content.ReadFromJsonAsync<ValidateResponse>(cancellationToken: ct);
        if (payload is null)
            return Result.Fail<bool>(ErrorsCodes.QatarResidentVerificationFailed);
        if(!(payload.ValidateCustomerResponse?.Result?.StatusMessage?.Equals("Success", StringComparison.InvariantCultureIgnoreCase) ?? false))
            return Result.Fail<bool>(ErrorsCodes.QatarResidentVerificationFailed);

        return Result.Ok(true);
        
        Uri BuildCustomerValidationUri(string qid, string mobileNo)
        {
            var query = $"QID={WebUtility.UrlEncode(qid)}&MobileNo={WebUtility.UrlEncode(mobileNo)}";
            return new Uri($"{_settings.VerificationPath}?{query}", UriKind.Relative);
        }
    }
    
    private async Task<IResult<MOIAuthResponse?>> AuthenticateAsync(CancellationToken ct = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post,_settings.AuthenticationPath) {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "password",
                    ["username"]   = _settings.Username,
                    ["password"]   = _settings.Password
                })
            };
            using var response = await httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();
            var payload = await response.Content.ReadFromJsonAsync<MOIAuthResponse>(cancellationToken: ct);
            return Result.Ok(payload);
        }
        catch(Exception ex)
        {
            logger.Error(ex, "QatarResidentVerificationClient: Authentication failed.");
            return Result.Fail<MOIAuthResponse?>(new Error("QatarResidentVerificationClient: Authentication failed.").CausedBy(ex));
        }
    }
}


