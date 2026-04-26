using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Notifications;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Domain.Entities.Notification;

using Tawtheef.Notifications.Interfaces;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class NotificationDispatcher(
    IServiceScopeFactory scopeFactory,
    TimeProvider time,
    IAppLogger logger)
    : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan StuckThreshold = TimeSpan.FromSeconds(60);
    private const int BatchSize = 25;
    private readonly IAppLogger _log = logger.ForContext(typeof(NotificationDispatcher));
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunCycleSafely(stoppingToken);
            await Task.Delay(PollInterval, stoppingToken);
        }
    }

    private async Task RunCycleSafely(CancellationToken ct)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();

            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var smsSender = scope.ServiceProvider.GetRequiredService<ISmsSender>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
            var pushSender = scope.ServiceProvider.GetRequiredService<IPushSender>();

            var repo = uow.GetEntityRepository<Notification>();
            var renderer = scope.ServiceProvider.GetRequiredService<IEmailTemplateRenderer>();
            var handlers = BuildHandlers(emailSender, smsSender, pushSender);

            // Recover any notifications stuck in Queued state
            await RecoverStuckQueued(repo, ct);

            var batch = await FetchPendingBatch(repo, ct);
            if (batch.Count == 0)
                return;

            foreach (var n in batch)
            {
                try
                {
                    await ProcessOne(n, handlers, renderer, ct);
                    await uow.SaveChangesAsync(ct);
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "Failed to save notification {Id} after processing", n.Id);

                    // Reload tracked entities to clear dirty state
                    await uow.Rollback();

                    // Reset to Pending directly in DB so it can be retried
                    await repo.DbSet
                        .Where(x => x.Id == n.Id && x.Status == NotificationStatus.Queued)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.Status, NotificationStatus.Pending)
                            .SetProperty(p => p.ProviderMessageId, (string?)null), ct);

                    // Break out — remaining batch items will be picked up next cycle
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Notification dispatcher cycle failed");
        }
    }

    private async Task RecoverStuckQueued(
        IGenericRepository<Notification> repo,
        CancellationToken ct)
    {
        var cutoff = time.GetUtcNow().DateTime - StuckThreshold;

        var recovered = await repo.DbSet
            .Where(n => n.Status == NotificationStatus.Queued && n.CreatedDate < cutoff)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Status, NotificationStatus.Pending)
                .SetProperty(p => p.ProviderMessageId, (string?)null), ct);

        if (recovered > 0)
            _log.Warning("Recovered {Count} stuck Queued notifications back to Pending", recovered);
    }

    private async Task<List<Notification>> FetchPendingBatch(
        IGenericRepository<Notification> repo,
        CancellationToken ct)
    {
        var now = time.GetUtcNow().DateTime;
        var pendingIds = await repo.DbSet
            .Where(n => n.Status == NotificationStatus.Pending && (n.NextRetryAt == null || n.NextRetryAt <= now))
            .OrderBy(n => n.CreatedDate)
            .Take(BatchSize)
            .Select(n => n.Id)
            .ToListAsync(ct);

        if (pendingIds.Count == 0)
            return new List<Notification>();

        var lockId = Guid.NewGuid().ToString();

        var updatedCount = await repo.DbSet
            .Where(n => pendingIds.Contains(n.Id) && n.Status == NotificationStatus.Pending)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Status, NotificationStatus.Queued)
                .SetProperty(p => p.ProviderMessageId, lockId), ct);

        if (updatedCount == 0)
            return new List<Notification>();

        return await repo.DbSet
            .Where(n => pendingIds.Contains(n.Id) && n.ProviderMessageId == lockId)
            .ToListAsync(ct);
    }

    private async Task ProcessOne(
        Notification n,
        IReadOnlyDictionary<NotificationChannel, Func<Notification, CancellationToken, Task<NotificationResponse>>> handlers,
        IEmailTemplateRenderer renderer,
        CancellationToken ct)
    {
        try
        {
            await EnsureRendered(n, renderer);

            if (!handlers.TryGetValue(n.Channel, out var handler))
            {
                n.MarkFailed("UNSUPPORTED_NOTIFICATION_CHANNEL");
                return;
            }

            var result = await handler(n, ct);
            ApplyResult(n, result);
        }
        catch (Exception ex)
        {
            ScheduleRetry(n, ex.Message);
            _log.Error(ex, "Notification {Id} failed during processing", n.Id);
        }
    }

    private async Task EnsureRendered(Notification n, IEmailTemplateRenderer renderer)
    {
        if (string.IsNullOrWhiteSpace(n.TemplateKey)) return;

        // 1. Default Subject from Template Metadata if missing
        if (string.IsNullOrWhiteSpace(n.Subject))
        {
            var defaultSubject = renderer.GetDefaultSubject(n.TemplateKey, n.Language);
            if (!string.IsNullOrWhiteSpace(defaultSubject))
            {
                typeof(Notification).GetProperty(nameof(Notification.Subject))?
                    .SetValue(n, defaultSubject);
            }
        }

        // 2. Render Body if missing
        if (string.IsNullOrWhiteSpace(n.Body))
        {
            try
            {
                string content;
                if (n.Channel == NotificationChannel.Email)
                {
                    // For Email, we render HTML into Body and Text into PlainTextBody
                    var html = await renderer.RenderHtmlAsync(n.TemplateKey, n.PayloadJson ?? "{}", n.Language);
                    var text = await renderer.RenderTextAsync(n.TemplateKey, n.PayloadJson ?? "{}", n.Language);

                    typeof(Notification).GetProperty(nameof(Notification.PlainTextBody))?.SetValue(n, text);

                    content = html; // for subject extraction logic below
                }
                else
                {
                    // For other channels (SMS, Push), we just render text
                    content = await renderer.RenderTextAsync(n.TemplateKey, n.PayloadJson ?? "{}", n.Language);
                    content = content.Trim();
                }

                // If content starts with "Subject:", extract it and remove from body (legacy support)
                if (content.StartsWith("Subject:", StringComparison.OrdinalIgnoreCase))
                {
                    var lines = content.Split(['\r', '\n'], 2, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > 0)
                    {
                        var subjectLine = lines[0][8..].Trim();
                        typeof(Notification).GetProperty(nameof(Notification.Subject))?
                            .SetValue(n, subjectLine);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to render template {Key} for notification {Id}", n.TemplateKey, n.Id);
            }
        }
    }

    private void ApplyResult(Notification n, NotificationResponse result)
    {
        if (result.Ok)
        {
            // Truncate to match ProviderMessageId MaxLength(100)
            var providerId = result.ProviderId?.Length > 100
                ? result.ProviderId[..100]
                : result.ProviderId;
            n.MarkSent(providerId, time.GetUtcNow().DateTime);
            return;
        }

        var msg = (result.Errors?.Any() == true)
            ? string.Join(", ", result.Errors.Select(e => e.Message))
            : "UNKNOWN_ERROR";

        ScheduleRetry(n, msg);
    }

    private void ScheduleRetry(Notification n, string error)
    {
        // Exponential backoff: 1 min, 2 min, 4 min, 8 min...
        var nextRetryDelay = TimeSpan.FromMinutes(Math.Pow(2, n.RetryCount));
        var nextRetryAt = time.GetUtcNow().DateTime.Add(nextRetryDelay);

        n.MarkFailed(error, nextRetryAt);

        if (n.Status == NotificationStatus.Failed)
        {
            _log.Error("Notification {Id} failed permanently after {Retries} retries. Error: {Error}", n.Id, n.RetryCount, error);
        }
        else
        {
            _log.Warning("Notification {Id} failed attempt {Attempt}. Next retry scheduled at {NextRetry}. Error: {Error}", n.Id, n.RetryCount, nextRetryAt, error);
        }
    }

    private static IReadOnlyDictionary<NotificationChannel, Func<Notification, CancellationToken, Task<NotificationResponse>>> BuildHandlers(
        IEmailSender emailSender,
        ISmsSender smsSender,
        IPushSender pushSender)
    {
        return new Dictionary<NotificationChannel, Func<Notification, CancellationToken, Task<NotificationResponse>>>
        {
            [NotificationChannel.Email] = emailSender.SendAsync,

            [NotificationChannel.Sms] = async (n, ct) =>
            {
                if (string.IsNullOrWhiteSpace(n.ToAddress) || string.IsNullOrWhiteSpace(n.Body))
                    return Failure("SMS_MISSING_TO_OR_BODY");

                return await smsSender.SendAsync(n.ToAddress, n.Body, ct);
            },

            [NotificationChannel.Push] = async (n, ct) =>
            {
                if (n.UserId is null)
                    return Failure("PUSH_USER_ID_NOT_SET");

                if (string.IsNullOrWhiteSpace(n.Body))
                    return Failure("PUSH_MISSING_BODY");

                return await pushSender.SendAsync(
                    n.UserId.Value,
                    n.Subject ?? string.Empty,
                    n.Body,
                    ct);
            },

            [NotificationChannel.InApp] = (_, _) =>
                Task.FromResult(new NotificationResponse(true, null, null))
        };
    }

    private static NotificationResponse Failure(string code)
        => NotificationResponse.Failure(new Error(code));
}
