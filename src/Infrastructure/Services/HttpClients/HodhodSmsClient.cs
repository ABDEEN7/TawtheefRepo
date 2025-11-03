using System.Web;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Domain.Configurations;

namespace Tawtheef.Infrastructure.Services.HttpClients;

public sealed class HodhodSmsClient : ISmsGatewayClient
{
    private readonly HttpClient _http;
    private readonly HodhodSmsSettings _opt;

    public HodhodSmsClient(HttpClient http, IOptions<HodhodSmsSettings> opt)
    {
        _http = http;
        _opt  = opt.Value;
        _http.BaseAddress ??= new Uri(_opt.BaseUrl);
        _http.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<Result<string>> SmsPushAsync(string mobile, string message, CancellationToken ct)
    {
        var url = $"SMSPush?ApplicationID={Url(_opt.ApplicationId)}&Password={Url(_opt.Password)}" +
                  $"&MobileNumber={Url(mobile)}&MessageText={Url(message)}" +
                  $"&ConfirmDelivery={_opt.ConfirmDelivery}&Priority={_opt.Priority}";
        var resp = await _http.GetAsync(url, ct);
        if (!resp.IsSuccessStatusCode) return Result.Failure<string>($"SMSPush failed: {(int)resp.StatusCode}");
        var payload = await resp.Content.ReadAsStringAsync(ct);
        return Result.Success(payload); // gateway-specific success body ignored
    }

    public async Task<Result<string>> GenerateOtpAsync(string mobile, CancellationToken ct)
    {
        var url = $"GenerateOTP?ApplicationID={Url(_opt.ApplicationId)}&Password={Url(_opt.Password)}&MobileNumber={Url(mobile)}";
        var resp = await _http.GetAsync(url, ct);
        if (!resp.IsSuccessStatusCode) return Result.Failure<string>($"GenerateOTP failed: {(int)resp.StatusCode}");
        var payload = await resp.Content.ReadAsStringAsync(ct);

        // Many .asmx endpoints return: <string xmlns="...">OTP_REF</string> or raw text
        var reference = ExtractStringValue(payload);
        return string.IsNullOrWhiteSpace(reference)
            ? Result.Failure<string>("Empty OTP reference returned.")
            : Result.Success(reference);
    }

    public async Task<Result> ValidateOtpAsync(string otpValue, string otpReference, CancellationToken ct)
    {
        var url = $"ValidateOTP?ApplicationID={Url(_opt.ApplicationId)}&Password={Url(_opt.Password)}" +
                  $"&OTPValue={Url(otpValue)}&OTPReference={Url(otpReference)}";
        var resp = await _http.GetAsync(url, ct);
        if (!resp.IsSuccessStatusCode) return Result.Failure($"ValidateOTP failed: {(int)resp.StatusCode}");
        var payload = await resp.Content.ReadAsStringAsync(ct);

        // Treat any non-empty "true/OK" or SOAP <string>True</string> as success.
        var s = payload.Trim().ToLowerInvariant();
        var ok = s.Contains("true") || s.Contains("ok") || s.Contains(">true<");
        return ok ? Result.Success() : Result.Failure("Invalid OTP.");
    }

    private static string Url(string v) => HttpUtility.UrlEncode(v);

    private static string ExtractStringValue(string payload)
    {
        // If SOAP-ish: <string ...>VALUE</string>
        var start = payload.IndexOf(">", StringComparison.Ordinal);
        var end   = payload.LastIndexOf("<", StringComparison.Ordinal);
        if (start >= 0 && end > start) return payload[(start + 1)..end].Trim();
        return payload.Trim();
    }
}
