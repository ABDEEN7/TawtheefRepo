using System.Collections.Concurrent;
using System.Net.Sockets;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;
using Polly;
using Serilog;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Domain.Configurations.Settings;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class MailKitEmailTransport : IEmailTransport, IDisposable
{
    private readonly EmailSettings _settings;
    private readonly AsyncPolicy _resiliencePolicy;
    private readonly ConcurrentBag<SmtpClient> _clientPool = new();
    private readonly int _poolSize;
    private readonly ILogger _log;

    private const int DefaultTimeoutMs = 60000;
    private const int MaxRetryAttempts = 3;
    private const int CircuitBreakerFailureThreshold = 5;
    private static readonly TimeSpan CircuitBreakerDuration = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan OperationTimeout = TimeSpan.FromSeconds(60);

    public MailKitEmailTransport(IOptions<EmailSettings> settings, ILogger log)
    {
        _settings = settings.Value;
        _poolSize = Math.Max(1, _settings.MaxSmtpClients);
        _log = log.ForContext<MailKitEmailTransport>();

        _resiliencePolicy = BuildResiliencePolicy();
    }

    public Task SendAsync(EmailEnvelope envelope, CancellationToken ct = default) =>
        _resiliencePolicy.ExecuteAsync(token => ExecuteSendAsync(envelope, token), ct);

    private async Task ExecuteSendAsync(EmailEnvelope envelope, CancellationToken ct)
    {
        var client = await GetClientAsync(ct);
        var keepClient = true;

        try
        {
            using var message = BuildMimeMessage(envelope);
            await client.SendAsync(message, ct);

            _log.Information(
                "SMTP send succeeded (to={ToCount}, subject={Subject})",
                envelope.To.Count,
                Truncate(envelope.Subject, 100)
            );
        }
        catch (SmtpCommandException sce) when (IsPermanent(sce))
        {
            keepClient = false;
            _log.Error(
                sce,
                "Permanent SMTP error: status={StatusCode}, subject={Subject}",
                sce.StatusCode,
                Truncate(envelope.Subject, 100)
            );
            throw;
        }
        catch (Exception ex)
        {
            keepClient = false;
            _log.Error(
                ex,
                "SMTP send failed (subject={Subject}, to={ToCount})",
                Truncate(envelope.Subject, 100),
                envelope.To.Count
            );

            TryDisposeClient(client);
            throw;
        }
        finally
        {
            if (keepClient)
                ReturnClient(client);
            else
                await TryDisconnectClientAsync(client, ct);
        }
    }

    private async Task<SmtpClient> GetClientAsync(CancellationToken ct)
    {
        if (_clientPool.TryTake(out var pooledClient) && pooledClient.IsConnected)
            return pooledClient;

        var client = CreateSmtpClient();

        try
        {
            await ConnectAsync(client, ct);
            return client;
        }
        catch
        {
            client.Dispose();
            throw;
        }
    }

    private SmtpClient CreateSmtpClient()
    {
        var client = new SmtpClient
        {
            Timeout = DefaultTimeoutMs
        };

        // IMPORTANT:
        // We are intentionally doing "plain SMTP" like System.Net.Mail on port 25.
        // No TLS => no certificate validation issues.

        return client;
    }

    private async Task ConnectAsync(SmtpClient client, CancellationToken ct)
    {
        // Force the same behavior as:
        // new SmtpClient("smtp.edu.gov.qa", 25) with no SSL
        var socketOptions = SecureSocketOptions.None;

        await client.ConnectAsync(_settings.SmtpHost, 25, socketOptions, ct);

        // Do NOT authenticate (matches your working example).
        // If your SMTP later requires auth, enable it conditionally.
        // await client.AuthenticateAsync(_settings.EmailUser, _settings.EmailPass, ct);

        _log.Information("SMTP connected (host={Host}, port={Port}, ssl={Ssl})",
            _settings.SmtpHost, 25, socketOptions);
    }

    private void ReturnClient(SmtpClient client)
    {
        if (_clientPool.Count < _poolSize && client.IsConnected)
            _clientPool.Add(client);
        else
            client.Dispose();
    }

    private static async Task TryDisconnectClientAsync(SmtpClient client, CancellationToken ct)
    {
        try
        {
            if (client.IsConnected)
                await client.DisconnectAsync(true, ct);
        }
        catch
        {
            // ignored
        }
    }

    private static void TryDisposeClient(SmtpClient client)
    {
        try { client.Dispose(); } catch { /* ignored */ }
    }

    private AsyncPolicy BuildResiliencePolicy()
    {
        var retryPolicy = BuildRetryPolicy();
        var circuitBreakerPolicy = BuildCircuitBreakerPolicy();
        var timeoutPolicy = Policy.TimeoutAsync(OperationTimeout);

        return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);
    }

    private AsyncPolicy BuildRetryPolicy()
    {
        return Policy
            .Handle<IOException>()
            .Or<SocketException>()
            .Or<SmtpProtocolException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: MaxRetryAttempts,
                sleepDurationProvider: attempt =>
                    TimeSpan.FromMilliseconds(500 * (int)Math.Pow(2, attempt)) +
                    TimeSpan.FromMilliseconds(Random.Shared.Next(0, 200)),
                onRetry: (exception, delay, attempt, _) =>
                {
                    _log.Warning(
                        exception,
                        "SMTP transient error (attempt {Attempt}) – retrying in {Delay} ms",
                        attempt,
                        delay.TotalMilliseconds
                    );
                });
    }

    private AsyncPolicy BuildCircuitBreakerPolicy()
    {
        return Policy
            .Handle<Exception>(IsTransient)
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: CircuitBreakerFailureThreshold,
                durationOfBreak: CircuitBreakerDuration,
                onBreak: (exception, duration) =>
                {
                    _log.Error(exception, "SMTP circuit opened for {Duration}s", duration.TotalSeconds);
                },
                onReset: () => _log.Information("SMTP circuit closed, sending resumes"),
                onHalfOpen: () => _log.Information("SMTP circuit half-open, next call is a trial"));
    }

    private MimeMessage BuildMimeMessage(EmailEnvelope envelope)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("Tawtheef", _settings.EmailUser));

        foreach (var recipient in envelope.To)
            message.To.Add(MailboxAddress.Parse(recipient));

        if (envelope.Cc is not null)
        {
            foreach (var cc in envelope.Cc)
                message.Cc.Add(MailboxAddress.Parse(cc));
        }

        message.Subject = envelope.Subject;

        var bodyBuilder = new BodyBuilder
        {
            TextBody = envelope.PlainTextBody,
            HtmlBody = envelope.HtmlBody
        };

        AttachInlineLogoIfConfigured(bodyBuilder);

        message.Headers.Add("X-Mailer", "Tawtheef");
        message.Headers.Add("X-Priority", "3");

        message.Body = bodyBuilder.ToMessageBody();
        return message;
    }

    private void AttachInlineLogoIfConfigured(BodyBuilder bodyBuilder)
    {
        if (string.IsNullOrEmpty(_settings.LogoPath))
            return;

        var logoCid = MimeUtils.GenerateMessageId("Tawtheef");

        var logo = new MimePart("image", "png")
        {
            Content = new MimeContent(File.OpenRead(_settings.LogoPath)),
            ContentId = logoCid,
            ContentTransferEncoding = ContentEncoding.Base64,
            ContentDisposition = new ContentDisposition(ContentDisposition.Inline)
        };

        logo.ContentType.Name = null;
        logo.ContentDisposition.FileName = null;

        bodyBuilder.LinkedResources.Add(logo);

        if (!string.IsNullOrEmpty(bodyBuilder.HtmlBody))
            bodyBuilder.HtmlBody = bodyBuilder.HtmlBody.Replace("logo@tawtheef", $"cid:{logoCid}");
    }

    private static bool IsTransient(Exception ex) =>
        ex is IOException or SocketException or SmtpProtocolException or TaskCanceledException;

    private static bool IsPermanent(SmtpCommandException sce) =>
        (int)sce.StatusCode is >= 500 and < 600;

    private static string Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return value.Length <= maxLength ? value : value[..maxLength] + "…";
    }

    public void Dispose()
    {
        while (_clientPool.TryTake(out var client))
            TryDisposeClient(client);
    }
}
