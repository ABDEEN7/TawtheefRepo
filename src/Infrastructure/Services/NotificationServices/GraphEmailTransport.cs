using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Polly;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.ValueObjects;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class GraphEmailTransport : IEmailTransport
{
    private readonly IGraphMailer _graphMailer;
    private readonly GraphEmailSettings _settings;
    private readonly AppConfigSettings _appConfig;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAppLogger _log;

    private readonly AsyncPolicy _resiliencePolicy;

    public GraphEmailTransport(
        IGraphMailer graphMailer,
        IOptions<GraphEmailSettings> settings,
        IOptions<AppConfigSettings> appConfig,
        IHttpClientFactory httpClientFactory,
        IAppLogger log)
    {
        _graphMailer = graphMailer;
        _settings = settings.Value;
        _appConfig = appConfig.Value;
        _httpClientFactory = httpClientFactory;
        _log = log.ForContext(typeof(GraphEmailTransport));

        _resiliencePolicy = BuildResiliencePolicy();
    }

    public Task SendAsync(EmailEnvelope envelope, CancellationToken ct = default) =>
        _resiliencePolicy.ExecuteAsync(token => ExecuteSendAsync(envelope, token), ct);

    private async Task ExecuteSendAsync(EmailEnvelope envelope, CancellationToken ct)
    {
#if DEBUG
        _log.Information(
            "GraphEmailTransport DEBUG -> toCount={ToCount} ccCount={CcCount} subject={Subject} hasHtml={HasHtml} hasText={HasText}",
            envelope.To.Count,
            envelope.Cc?.Count ?? 0,
            Truncate(envelope.Subject, 120),
            !string.IsNullOrWhiteSpace(envelope.HtmlBody),
            !string.IsNullOrWhiteSpace(envelope.PlainTextBody));
#endif
        var htmlBody = envelope.HtmlBody;
        var attachments = new List<GraphMailAttachment>();

        if (hasHtml && htmlBody!.Contains("logo@careers"))
        {
            var result = await AttachLogoSmartAsync(htmlBody, ct);
            htmlBody = result.html;
            if (result.logo != null) attachments.Add(result.logo);
        }

        var request = new GraphMailRequest
        {
            Subject = envelope.Subject,
            HtmlBody = hasHtml ? htmlBody : null,
            TextBody = hasHtml ? null : envelope.PlainTextBody,
            To = envelope.To.ToArray(),
            Cc = envelope.Cc?.ToArray(),
            Attachments = attachments.Any() ? attachments : null
        };

        await _graphMailer.SendAsync(request, ct);

        _log.Information("GraphEmailTransport succeeded (to={ToCount}, subject={Subject})",
            envelope.To.Count,
            Truncate(envelope.Subject, 100));
    }

    private async Task<(string html, GraphMailAttachment? logo)> AttachLogoSmartAsync(string html, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(html))
            return (html, null);

        // 1. Try local file (Attachment)
        if (!string.IsNullOrWhiteSpace(_settings.LogoPath))
        {
            var path = _settings.LogoPath;
            var absolutePath = Path.IsPathRooted(path) ? path : Path.Combine(Directory.GetCurrentDirectory(), path);

            if (!File.Exists(absolutePath))
                absolutePath = Path.Combine(AppContext.BaseDirectory, path);

            if (File.Exists(absolutePath))
            {
                try
                {
                    var bytes = await File.ReadAllBytesAsync(absolutePath, ct);
                    var cid = "logo_cid_" + Guid.NewGuid().ToString("N")[..6];
                    
                    var logoAttachment = new GraphMailAttachment
                    {
                        Name = "logo.jpg",
                        ContentType = "image/jpeg",
                        ContentBytes = bytes,
                        ContentId = cid,
                        IsInline = true
                    };

                    html = html.Replace("logo@careers", $"cid:{cid}");
                    return (html, logoAttachment);
                }
                catch (Exception ex)
                {
                    _log.Warning(ex, "Failed to read logo from {Path} for email insertion", absolutePath);
                }
            }
        }

        // 2. Fallback to URL
        if (!string.IsNullOrWhiteSpace(_settings.LogoUrl))
        {
            var baseUrl = _settings.FrontendBaseUrl ?? _appConfig.FrontendUrl;
            var logoUrl = CombineUrl(baseUrl, _settings.LogoUrl);
            html = html.Replace("logo@careers", logoUrl);
        }

        return (html, null);
    }

    private AsyncPolicy BuildResiliencePolicy()
    {
        var retry = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, attempt =>
                TimeSpan.FromMilliseconds(500 * Math.Pow(2, attempt)));

        return retry;
    }

    private static string Truncate(string? value, int maxLength)
        => string.IsNullOrEmpty(value)
            ? string.Empty
            : value.Length <= maxLength
                ? value
                : value[..maxLength] + "…";

    public static string CombineUrl(string baseUrl, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("BaseUrl is required", nameof(baseUrl));

        if (string.IsNullOrWhiteSpace(relativePath))
            return baseUrl;

        return new Uri(new Uri(baseUrl.TrimEnd('/') + "/"), relativePath.TrimStart('/')).ToString();
    }
}
