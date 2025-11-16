using System.Collections.Concurrent;
using System.Net;
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
    private readonly EmailSettings _cfg;
    private readonly AsyncPolicy _policy;
    private readonly ConcurrentBag<SmtpClient> _pool = new();
    private readonly int _poolSize;
    private readonly ILogger _log;

    public MailKitEmailTransport(IOptions<EmailSettings> cfg, ILogger log)
    {
        _cfg = cfg.Value;
        _poolSize = Math.Max(1, _cfg.MaxSmtpClients);
        _log = log.ForContext<MailKitEmailTransport>();

        var retry = Policy
            .Handle<IOException>()
            .Or<SocketException>()
            .Or<SmtpProtocolException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                3,
                i => TimeSpan.FromMilliseconds(500 * (int)Math.Pow(2, i)) 
                     + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 200)),
                (ex, delay, attempt, _) =>
                    _log.Warning(ex, "SMTP transient error (attempt {Attempt}) – retrying in {Delay} ms", attempt, delay.TotalMilliseconds));

        var breaker = Policy
            .Handle<Exception>(IsTransient)
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(60),
                onBreak: (ex, ts) => _log.Error(ex, "SMTP circuit opened for {Duration}s", ts.TotalSeconds),
                onReset: () => _log.Information("SMTP circuit closed, sending resumes"),
                onHalfOpen: () => _log.Information("SMTP circuit half-open, next call is a trial"));

        var timeout = Policy.TimeoutAsync(TimeSpan.FromSeconds(15));

        _policy = Policy.WrapAsync(retry, breaker, timeout);
    }

    public Task SendAsync(EmailEnvelope env, CancellationToken ct = default) =>
        _policy.ExecuteAsync(async ct2 =>
        {
            //var sw = Stopwatch.StartNew();
            var client = await GetClientAsync(ct2);
            var keepClient = true;

            try
            {
                using var msg = BuildMimeMessage(env);
#if !DEBUG
                await client.SendAsync(msg, ct2);
#endif

                // _log.Information("SMTP send ok in {Elapsed} ms (to={ToCount}, subj={Subject})",
                //     sw.ElapsedMilliseconds, env.To.Count, Truncate(env.Subject, 100));
            }
            catch (SmtpCommandException sce) when (IsPermanent(sce))
            {
                keepClient = false; // drop on protocol-level permanent error
                // _log.Error(sce, "Permanent SMTP error: status={StatusCode}, subj={Subject}",
                //     sce.StatusCode, Truncate(env.Subject, 100));
                throw;
            }
            catch (Exception ex)
            {
                keepClient = false; // socket/protocol broken; don’t reuse
                _log.Error(ex, "SMTP send failed for subj={Subject}, to={ToCount}",
                    Truncate(env.Subject, 100), env.To.Count);
                try { client.Dispose(); } catch { /* ignore */ }
                throw;
            }
            finally
            {
                if (keepClient)
                    ReturnClient(client);
                else
                    try { if (client.IsConnected) await client.DisconnectAsync(true, ct2); } catch { /* ignore */ }
            }
        }, ct);
    
    private async Task<SmtpClient> GetClientAsync(CancellationToken ct)
    {
        if (_pool.TryTake(out var pooled) && pooled.IsConnected)
            return pooled;

        var client = new SmtpClient { Timeout = 15000 };

#if DEBUG
        client.CheckCertificateRevocation = false; // dev only
#endif

        // Optional: prefer IPv4 by ordering DNS results, but let MailKit connect itself.
        // (If you *must* choose an IP manually, do NOT dispose the socket you pass in.)
        var ips = await Dns.GetHostAddressesAsync(_cfg.SmtpHost, ct);
        var orderedIps = ips.OrderBy(ip => ip.AddressFamily == AddressFamily.InterNetwork ? 0 : 1).ToList();

        Exception? last = null;
        foreach (var ip in orderedIps)
        {
            try
            {
                // Simpler & safe: let MailKit open the socket
                await client.ConnectAsync(
                    _cfg.SmtpHost,                      // keeps SNI/hostname validation
                    _cfg.SmtpPort,
                    _cfg.EnableSsl
                        ? (_cfg.SmtpPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls)
                        : SecureSocketOptions.None,
                    ct);

                await client.AuthenticateAsync(_cfg.EmailUser, _cfg.EmailPass, ct);

                _log.Information("SMTP connected/authenticated to {Host}:{Port} ({Ip})",
                    _cfg.SmtpHost, _cfg.SmtpPort, ip);

                return client;
            }
            catch (Exception ex)
            {
                last = ex;
                try { if (client.IsConnected) await client.DisconnectAsync(true, ct); } catch { /* ignore */ }
                _log.Warning(ex, "SMTP connect/auth failed via {Ip}:{Port}", ip, _cfg.SmtpPort);
            }
        }

        client.Dispose();
        throw last ?? new TimeoutException("SMTP connect/auth failed for all IPs");
    }

    private void ReturnClient(SmtpClient c)
    {
        if (_pool.Count < _poolSize && c.IsConnected)
            _pool.Add(c);
        else
            c.Dispose();
    }

    private static bool IsTransient(Exception ex) =>
        ex is IOException or SocketException or SmtpProtocolException or TaskCanceledException;

    private static bool IsPermanent(SmtpCommandException sce) =>
        (int)sce.StatusCode >= 500 && (int)sce.StatusCode < 600;

    private MimeMessage BuildMimeMessage(EmailEnvelope env)
    {
        var message = new MimeMessage();
        // From / To / Cc
        message.From.Add(new MailboxAddress("Tawtheef", _cfg.EmailUser));
        foreach (var r in env.To) message.To.Add(MailboxAddress.Parse(r));
        foreach (var cc in env.Cc ?? []) message.Cc.Add(MailboxAddress.Parse(cc));

        message.Subject = env.Subject;

        // multipart/alternative (text + html)
        var builder = new BodyBuilder
        {
            TextBody = env.PlainTextBody,
            HtmlBody = env.HtmlBody
        };

        // Build a CID resource with no filename to reduce showing as an attachment
        if (!string.IsNullOrEmpty(_cfg.LogoPath))
        {
            var logoCid = MimeUtils.GenerateMessageId("Tawtheef");
            var logo = new MimePart("image", "png")
            {
                Content = new MimeContent(File.OpenRead(_cfg.LogoPath)),
                ContentId = logoCid,
                ContentTransferEncoding = ContentEncoding.Base64,
                ContentDisposition = new ContentDisposition(ContentDisposition.Inline) // inline, not attachment
            };
            // IMPORTANT: remove names/filenames
            logo.ContentType.Name = null;
            logo.ContentDisposition.FileName = null;
            builder.LinkedResources.Add(logo);
            // Replace the template URL with CID
            builder.HtmlBody = builder.HtmlBody!.Replace("logo@tawtheef", $"cid:{logoCid}");
        }

        // Deliverability hints (optional)
        message.Headers.Add("X-Mailer", "Tawtheef");
        message.Headers.Add("X-Priority", "3");

        message.Body = builder.ToMessageBody();
        return message;
    }

    public void Dispose()
    {
        while (_pool.TryTake(out var c))
        {
            try { c.Dispose(); }
            catch { /* ignored */ }
        }
    }

    private static string Truncate(string? s, int len) =>
        string.IsNullOrEmpty(s) ? string.Empty : (s.Length <= len ? s : s[..len] + "…");
}
