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
    private readonly IAppLogger _log;

    private readonly AsyncPolicy _resiliencePolicy;

    public GraphEmailTransport(
        IGraphMailer graphMailer,
        IOptions<GraphEmailSettings> settings,
        IOptions<AppConfigSettings> appConfig,
        IAppLogger log)
    {
        _graphMailer = graphMailer;
        _settings = settings.Value;
        _appConfig = appConfig.Value;
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
        var hasHtml = !string.IsNullOrWhiteSpace(envelope.HtmlBody);

        var htmlBody = envelope.HtmlBody;
        if (hasHtml)
            htmlBody = AttachLogoSmart(htmlBody!);

        var request = new GraphMailRequest
        {
            Subject = envelope.Subject,
            HtmlBody = hasHtml ? htmlBody : null,
            TextBody = hasHtml ? null : envelope.PlainTextBody,
            To = envelope.To.ToArray(),
            Cc = envelope.Cc?.ToArray()
        };

        await _graphMailer.SendAsync(request, ct);

        _log.Information("GraphEmailTransport succeeded (to={ToCount}, subject={Subject})",
            envelope.To.Count,
            Truncate(envelope.Subject, 100));
    }

    private string AttachLogoSmart(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return html;

        if (!string.IsNullOrWhiteSpace(_settings.LogoUrl))
        {
            var baseUrl = _settings.FrontendBaseUrl ?? _appConfig.FrontendUrl;
            var logoUrl = CombineUrl(baseUrl, _settings.LogoUrl);
            return html.Replace("logo@careers", logoUrl);
        }

        return html;
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
