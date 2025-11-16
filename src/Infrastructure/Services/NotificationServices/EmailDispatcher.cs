using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Domain.Configurations.Settings;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class EmailDispatcher(
    IEmailQueue queue, IEmailTransport transport,
    IOptions<EmailDispatcherSettings> opt, ILogger log) : BackgroundService
{
    private readonly EmailDispatcherSettings _cfg = opt.Value;
    private int _activeWorkers;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var reader = ((EmailQueue)queue).Reader;

        // Spin up workers
        var workers = Enumerable.Range(0, Math.Max(1, _cfg.Workers))
            .Select(i => Task.Run(() => WorkerLoop(i, reader, stoppingToken), stoppingToken))
            .ToArray();

        try
        {
            await Task.WhenAll(workers);
        }
        catch (OperationCanceledException)
        {
             /* normal on shutdown */
        }
    }

    private async Task WorkerLoop(int workerId, ChannelReader<EmailEnvelope> reader, CancellationToken ct)
    {
        Interlocked.Increment(ref _activeWorkers);
        var consecutiveFailures = 0;

        try
        {
            while (!ct.IsCancellationRequested)
            {
                // Wait for data
                if (!await reader.WaitToReadAsync(ct))
                {
                    // Channel completed
                    break;
                }

                while (reader.TryRead(out var env))
                {
                    var sw = Stopwatch.StartNew();
                    try
                    {
                        // Per-envelope attempts (poison guard). Transport may also have internal retries.
                        var attempt = 0;
                        for (; attempt < Math.Max(1, _cfg.MaxAttemptsPerEnvelope); attempt++)
                        {
                            try
                            {
                                await transport.SendAsync(env, ct);
                                break; // success
                            }
                            catch when (attempt + 1 < _cfg.MaxAttemptsPerEnvelope)
                            {
                                // brief, bounded backoff between attempts
                                var delay = _cfg.BackoffMs * (attempt + 1) + Random.Shared.Next(0, 150);
                                await Task.Delay(delay, ct);
                            }
                        }

                        if (attempt == _cfg.MaxAttemptsPerEnvelope)
                            throw new Exception("Exceeded MaxAttemptsPerEnvelope");

                        consecutiveFailures = 0; // reset on success
                        log.Information("Email sent by worker {Worker} in {Ms} ms; to={ToCount}; subj={Subject}",
                            workerId, sw.ElapsedMilliseconds, env.To.Count, Trunc(env.Subject, 120));
                    }
                    catch (Exception ex)
                    {
                        consecutiveFailures++;
                        log.Error(ex, "Email send failed by worker {Worker}; to={ToCount}; subj={Subject}; consecutiveFailures={CF}",
                            workerId, env.To.Count, Trunc(env.Subject, 120), consecutiveFailures);

                        // Optional: dead-letter hook after repeated failures
                        if (consecutiveFailures >= _cfg.MaxConsecutiveFailuresBeforeBackoff)
                        {
                            var delay = _cfg.BackoffMs * consecutiveFailures + Random.Shared.Next(0, 300);
                            await Task.Delay(delay, ct);
                            // Keep going; transport-level circuit breaker should also kick in.
                        }
                    }
                }
            }
        }
        finally
        {
            Interlocked.Decrement(ref _activeWorkers);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // Optionally, drain the queue for a short time before stopping completely
        if (_cfg.DrainOnStop)
        {
            var deadline = DateTime.UtcNow + _cfg.MaxDrainDuration;
            while (_activeWorkers > 0 && DateTime.UtcNow < deadline)
                await Task.Delay(100, cancellationToken);
        }
        await base.StopAsync(cancellationToken);
    }

    private static string Trunc(string? s, int n) =>
        string.IsNullOrEmpty(s) ? string.Empty : (s.Length <= n ? s : s[..n] + "…");
}
