using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Notifications;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Domain.Entities.Notification;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class NotificationDispatcher(
    IServiceScopeFactory scopeFactory,
    TimeProvider time,
    IAppLogger logger)
    : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);
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
            var handlers = BuildHandlers(emailSender, smsSender, pushSender);

            var batch = await FetchPendingBatch(repo, ct);
            if (batch.Count == 0)
                return;

            foreach (var n in batch)
            {
                await ProcessOne(n, handlers, ct);
                await uow.SaveChangesAsync(ct);
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Notification dispatcher cycle failed");
        }
    }

    private static async Task<List<Notification>> FetchPendingBatch(
        IGenericRepository<Notification> repo,
        CancellationToken ct)
    {
        var pendingIds = await repo.DbSet
            .Where(n => n.Status == NotificationStatus.Pending)
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
        CancellationToken ct)
    {
        try
        {
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
            n.MarkFailed(ex.Message);
            _log.Error(ex, "Notification {Id} failed", n.Id);
        }
    }

    private void ApplyResult(Notification n, NotificationResponse result)
    {
        if (result.Ok)
        {
            n.MarkSent(result.ProviderId, time.GetUtcNow().DateTime);
            return;
        }

        var msg = (result.Errors?.Any() == true)
            ? string.Join(", ", result.Errors.Select(e => e.Message))
            : "UNKNOWN_ERROR";

        n.MarkFailed(msg);
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
