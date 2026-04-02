using System.Diagnostics;
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

        var correlationId = Guid.NewGuid().ToString("N");

        try
        {
            var token = await GetAccessTokenAsync(ct);

            var url = $"users/{Uri.EscapeDataString(_settings.FromAddress)}/sendMail";

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // MS Graph correlation headers (useful for investigation)
            httpRequest.Headers.TryAddWithoutValidation("client-request-id", correlationId);
            httpRequest.Headers.TryAddWithoutValidation("return-client-request-id", "true");

            var payload = BuildPayload(request);
            var json = JsonSerializer.Serialize(payload);

            if (IsDiagnosticsEnabled())
            {
                _log.Information(
                    "Graph DEBUG -> correlationId={CorrelationId} from={From} toCount={ToCount} ccCount={CcCount} subject={Subject} url={Url}",
                    correlationId,
                    _settings.FromAddress,
                    request.To.Count,
                    request.Cc?.Count ?? 0,
                    SafeTruncate(request.Subject, 120),
                    url);

                _log.Debug("Graph DEBUG payload correlationId={CorrelationId} payload={Payload}", correlationId, SafeTruncate(json, 2000));
            }

            httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var sw = Stopwatch.StartNew();
            using var response = await _http.SendAsync(httpRequest, ct);
            sw.Stop();

            var requestId = GetHeaderFirst(response, "request-id");
            var clientRequestId = GetHeaderFirst(response, "client-request-id");
            var diag = GetHeaderFirst(response, "x-ms-ags-diagnostic");
            var date = GetHeaderFirst(response, "Date");

            if (IsDiagnosticsEnabled())
            {
                _log.Information(
                    "Graph DEBUG <- correlationId={CorrelationId} status={Status} elapsedMs={ElapsedMs} request-id={RequestId} client-request-id={ClientRequestId} date={Date} diag={Diag}",
                    correlationId,
                    (int)response.StatusCode,
                    sw.ElapsedMilliseconds,
                    requestId,
                    clientRequestId,
                    date,
                    SafeTruncate(diag, 800));
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);

                _log.Error(
                    "Graph sendMail failed correlationId={CorrelationId} status={Status} request-id={RequestId} subject={Subject} body={Body}",
                    correlationId,
                    (int)response.StatusCode,
                    requestId,
                    SafeTruncate(request.Subject, 200),
                    SafeTruncate(errorBody, 4000));

                response.EnsureSuccessStatusCode();
            }

            _log.Information(
                "Graph sendMail succeeded correlationId={CorrelationId} request-id={RequestId} toCount={ToCount} subject={Subject}",
                correlationId,
                requestId,
                request.To.Count,
                SafeTruncate(request.Subject, 200));
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            _log.Warning("Graph sendMail canceled correlationId={CorrelationId}", correlationId);
            throw;
        }
        catch (Exception ex)
        {
            _log.Error(ex,
                "Graph sendMail exception correlationId={CorrelationId} from={From} subject={Subject}",
                correlationId,
                _settings.FromAddress,
                SafeTruncate(request.Subject, 200));
            throw;
        }
    }

    private object BuildPayload(GraphMailRequest request)
    {
        var isHtml = !string.IsNullOrWhiteSpace(request.HtmlBody);

        var attachments = request.Attachments?.Select(a => new Dictionary<string, object?>
        {
            ["@odata.type"] = "#microsoft.graph.fileAttachment",
            ["name"] = a.Name,
            ["contentType"] = a.ContentType,
            ["contentBytes"] = Convert.ToBase64String(a.ContentBytes),
            ["contentId"] = a.ContentId,
            ["isInline"] = a.IsInline
        });

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
                    .Select(x => new { emailAddress = new { address = x } }),
                attachments = attachments
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

            if (IsDiagnosticsEnabled())
                _log.Information("Graph token refresh DEBUG -> currentExpiry={Expiry}", _cachedTokenExpiry);

            _cachedToken = await _credential.GetTokenAsync(new TokenRequestContext(Scopes), ct);
            _cachedTokenExpiry = _cachedToken.ExpiresOn;

            if (IsDiagnosticsEnabled())
                _log.Information("Graph token acquired DEBUG -> expiresOn={ExpiresOn}", _cachedTokenExpiry);

            return _cachedToken.Token;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private static string? GetHeaderFirst(HttpResponseMessage response, string name)
        => response.Headers.TryGetValues(name, out var values)
            ? values.FirstOrDefault()
            : null;

    private bool IsDiagnosticsEnabled()
    {
#if DEBUG
        return true;
#else
        // إذا بدك تفعّلها Production عبر setting:
        // return _settings.EnableDiagnostics;
        return false;
#endif
    }

    private static string SafeTruncate(string? value, int maxLength)
        => string.IsNullOrEmpty(value)
            ? string.Empty
            : value.Length <= maxLength ? value : value[..maxLength] + "…";
}
