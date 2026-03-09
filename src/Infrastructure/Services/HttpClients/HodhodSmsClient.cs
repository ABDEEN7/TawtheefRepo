using System.Xml.Linq;
using FluentResults;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Domain.Configurations.Settings;

namespace Tawtheef.Infrastructure.Services.HttpClients;


public sealed class HodhodSmsClient(HttpClient http, IAppLogger logger,
    IOptions<HodhodSmsSettings> opt) : ISmsGatewayClient
{
    private const string SuccessStatus = "SUCCESS";
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
            logger.Error(ex, "SMSPush failed");
            return Result.Fail<string>($"SMSPush failed: HTTP error - {ex.Message}");
        }

        if (!resp.IsSuccessStatusCode)
        {
            logger.Error("SMSPush failed: {StatusCode}", (int)resp.StatusCode);
            return Result.Fail<string>($"SMSPush failed: {(int)resp.StatusCode}");
        }

        var payload = await resp.Content.ReadAsStringAsync(ct);
        return ParseXmlResponse(payload, mobile);
    }

    private IResult<string> ParseXmlResponse(string xml, string mobile)
    {
        try
        {
            var doc = XDocument.Parse(xml);
            var ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;

            var status = doc.Root?.Element(ns + "Status")?.Value;
            var returnCode = doc.Root?.Element(ns + "ReturnCode")?.Value;
            var mobileNumber = doc.Root?.Element(ns + "MobileNumber")?.Value;

            if (string.Equals(status, SuccessStatus, StringComparison.OrdinalIgnoreCase))
                return Result.Ok($"{returnCode}:{mobileNumber}");

            logger.Error("SMSPush returned non-success for {Mobile}: Status={Status}, ReturnCode={ReturnCode}",
                mobile, status, returnCode);

            return Result.Fail<string>($"SMSPush failed: Status={status}, ReturnCode={returnCode}");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "SMSPush: Failed to parse XML response for {Mobile}", mobile);
            return Result.Fail<string>($"SMSPush: XML parse error - {ex.Message}");
        }
    }
}
