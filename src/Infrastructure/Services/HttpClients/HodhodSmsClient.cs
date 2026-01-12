using FluentResults;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Domain.Configurations.Settings;

namespace Tawtheef.Infrastructure.Services.HttpClients;


public sealed class HodhodSmsClient(HttpClient http, IOptions<HodhodSmsSettings> opt) : ISmsGatewayClient
{
    private readonly HodhodSmsSettings _opt = opt.Value;

    public async Task<IResult<string>> SmsPushAsync(
        string mobile,
        string message,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(mobile))
            return Result.Fail<string>("SmsPush: Mobile number is required.");

        if (string.IsNullOrWhiteSpace(message))
            return Result.Fail<string>("SmsPush: Message text is required.");

        var query = new Dictionary<string, string?>
        {
            ["ApplicationID"]   = _opt.ApplicationId,
            ["Password"]        = _opt.Password,
            ["MobileNumber"]    = mobile,
            ["MessageText"]     = message,
            ["DefaultOtpMinutes"]     = _opt.DefaultOtpMinutes.ToString(),
            ["ConfirmDelivery"] = _opt.ConfirmDelivery.ToString(),
            ["Priority"]        = _opt.Priority.ToString()
        };

        var url = QueryHelpers.AddQueryString("SMSPush", query);

        HttpResponseMessage resp;
        try
        {
            resp = await http.GetAsync(url, ct);
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is HttpRequestException)
        {
            return Result.Fail<string>($"SMSPush failed: HTTP error - {ex.Message}");
        }

        if (!resp.IsSuccessStatusCode)
            return Result.Fail<string>($"SMSPush failed: {(int)resp.StatusCode}");

        var payload = await resp.Content.ReadAsStringAsync(ct);
        return Result.Ok(payload);
    }
}
