using System.Web;
using FluentResults;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Domain.Configurations.Settings;

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

    public async Task<IResult<string>> SmsPushAsync(string mobile, string message, CancellationToken ct)
    {
        var url = $"SMSPush?ApplicationID={Url(_opt.ApplicationId)}&Password={Url(_opt.Password)}" +
                  $"&MobileNumber={Url(mobile)}&MessageText={Url(message)}" +
                  $"&ConfirmDelivery={_opt.ConfirmDelivery}&Priority={_opt.Priority}";
        var resp = await _http.GetAsync(url, ct);
        if (!resp.IsSuccessStatusCode) return Result.Fail<string>($"SMSPush failed: {(int)resp.StatusCode}");
        var payload = await resp.Content.ReadAsStringAsync(ct);
        return Result.Ok(payload); // gateway-specific success body ignored
    }

    private static string Url(string v) => HttpUtility.UrlEncode(v);
}
