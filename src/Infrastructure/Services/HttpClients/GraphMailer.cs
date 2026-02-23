using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.ValueObjects;

namespace Tawtheef.Infrastructure.Services.HttpClients;

public sealed class GraphMailer : IGraphMailer
{
    private static readonly string[] Scopes = ["https://graph.microsoft.com/.default"];

    private readonly HttpClient _http;
    private readonly GraphEmailSettings _settings;
    private readonly IAppLogger _log;
    private readonly TokenCredential _credential;

    private readonly SemaphoreSlim _tokenLock = new(1, 1);
    private AccessToken _cachedToken;
    private DateTimeOffset _cachedTokenExpiry = DateTimeOffset.MinValue;

    public GraphMailer(
        HttpClient http,
        IOptions<GraphEmailSettings> settings,
        IAppLogger logger)
    {
        _http = http;
        _settings = settings.Value;
        _log = logger.ForContext(typeof(GraphMailer));

        _credential = new ClientSecretCredential(
            _settings.TenantId,
            _settings.ClientId,
            _settings.ClientSecret);
    }

    public async Task SendAsync(GraphMailRequest request, CancellationToken ct = default)
    {
        if (request.To.Count == 0)
            throw new ArgumentException("At least one recipient is required.", nameof(request));

        var token = await GetAccessTokenAsync(ct);
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = BuildPayload(request);
        var json = JsonSerializer.Serialize(payload);

        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        // because BaseAddress = https://graph.microsoft.com/v1.0/
        var url = $"users/{Uri.EscapeDataString(_settings.FromAddress)}/sendMail";

        using var response = await _http.PostAsync(url, content, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(ct);
            _log.Error("Graph sendMail failed. Status={Status} Subject={Subject} Body={Body}",
                (int)response.StatusCode,
                request.Subject,
                errorBody);

            response.EnsureSuccessStatusCode();
        }

        _log.Information("Graph sendMail succeeded. ToCount={Count} Subject={Subject}",
            request.To.Count,
            request.Subject);
    }

    private object BuildPayload(GraphMailRequest request)
    {
        var isHtml = !string.IsNullOrWhiteSpace(request.HtmlBody);

        return new
        {
            message = new
            {
                subject = request.Subject,
                body = new
                {
                    contentType = isHtml ? "HTML" : "Text",
                    content = isHtml ? request.HtmlBody : (request.TextBody ?? string.Empty)
                },
                toRecipients = request.To.Select(x => new { emailAddress = new { address = x } }),
                ccRecipients = (request.Cc ?? Array.Empty<string>())
                    .Select(x => new { emailAddress = new { address = x } })
            },
            saveToSentItems = _settings.SaveToSentItems
        };
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken ct)
    {
        if (_cachedTokenExpiry > DateTimeOffset.UtcNow.AddMinutes(2))
            return _cachedToken.Token;

        await _tokenLock.WaitAsync(ct);
        try
        {
            if (_cachedTokenExpiry > DateTimeOffset.UtcNow.AddMinutes(2))
                return _cachedToken.Token;

            _cachedToken = await _credential.GetTokenAsync(new TokenRequestContext(Scopes), ct);
            _cachedTokenExpiry = _cachedToken.ExpiresOn;
            return _cachedToken.Token;
        }
        finally
        {
            _tokenLock.Release();
        }
    }
}
