using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Recruitment.Common.Interfaces.Services.HttpClients;
using Application.Recruitment.Features.Authenticator.DTOs;
using FluentResults;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Infrastructure.Services.HttpClients;

public sealed class QatarResidentVerificationClient(
    HttpClient httpClient,
    IAppLogger logger,
    IOptions<QatarResidentOtpSettings> settings)
    : IQatarResidentVerificationClient
{
    private readonly QatarResidentOtpSettings _settings = settings.Value;

    public async Task<IResult<bool>> VerifyAsync(
        string qid,
        string phoneNumber,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(qid) ||
            string.IsNullOrWhiteSpace(phoneNumber))
        {
            return Result.Fail<bool>(
                ErrorsCodes.QatarResidentVerificationFailed);
        }

        var authResult = await AuthenticateAsync(ct);

        if (authResult.IsFailed)
            return Result.Fail<bool>(authResult.Errors);

        if (string.IsNullOrWhiteSpace(authResult.Value?.AccessToken))
        {
            return Result.Fail<bool>(
                ErrorsCodes.QatarResidentVerificationFailed);
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            BuildCustomerValidationUri(qid, phoneNumber));

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                authResult.Value.AccessToken);

        request.Headers.Add("UserName", _settings.GiveUserName);

        using var response = await httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(ct);

            logger.Error(
                "QatarResidentVerificationClient: Verification request failed. StatusCode: {StatusCode}. Body: {Body}",
                (int)response.StatusCode,
                content);

            return Result.Fail<bool>(
                response.StatusCode == HttpStatusCode.TooManyRequests
                    ? ErrorsCodes.QatarResidentVerificationTemporarilyUnavailable
                    : ErrorsCodes.QatarResidentVerificationFailed);
        }

        var payload = await response.Content
            .ReadFromJsonAsync<ValidateResponse>(cancellationToken: ct);

        var isVerified =
            payload?.ValidateCustomerResponse?.Result?.StatusMessage
                ?.Equals(
                    "Success",
                    StringComparison.OrdinalIgnoreCase)
            == true;

        return isVerified
            ? Result.Ok(true)
            : Result.Fail<bool>(
                ErrorsCodes.QatarResidentVerificationFailed);
    }

    private async Task<IResult<MOIAuthResponse?>> AuthenticateAsync(
        CancellationToken ct)
    {
        try
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                _settings.AuthenticationPath);

            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    ["grant_type"] = "password",
                    ["username"] = _settings.Username,
                    ["password"] = _settings.Password
                });

            using var response = await httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                logger.Error(
                    "QatarResidentVerificationClient: Authentication failed. StatusCode: {StatusCode}",
                    (int)response.StatusCode);

                return Result.Fail<MOIAuthResponse?>(
                    ErrorsCodes.QatarResidentVerificationFailed);
            }

            var payload = await response.Content
                .ReadFromJsonAsync<MOIAuthResponse>(
                    cancellationToken: ct);

            if (string.IsNullOrWhiteSpace(payload?.AccessToken))
            {
                logger.Error(
                    "QatarResidentVerificationClient: Authentication response does not contain an access token.");

                return Result.Fail<MOIAuthResponse?>(
                    ErrorsCodes.QatarResidentVerificationFailed);
            }

            return Result.Ok<MOIAuthResponse?>(payload);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(
                "QatarResidentVerificationClient: Authentication request failed. Error: {Error}",
                ex.Message);

            return Result.Fail<MOIAuthResponse?>(
                ErrorsCodes.QatarResidentVerificationFailed);
        }
    }

    private Uri BuildCustomerValidationUri(
        string qid,
        string phoneNumber)
    {
        var mobileNumber = NormalizeMobileNumber(phoneNumber);

        var query =
            $"QID={WebUtility.UrlEncode(qid)}" +
            $"&MobileNo={WebUtility.UrlEncode(mobileNumber)}";

        return new Uri(
            $"{_settings.VerificationPath}?{query}",
            UriKind.Relative);
    }

    private static string NormalizeMobileNumber(string phoneNumber)
    {
        return phoneNumber.StartsWith(
            "+974",
            StringComparison.Ordinal)
            ? phoneNumber[4..]
            : phoneNumber;
    }
}
