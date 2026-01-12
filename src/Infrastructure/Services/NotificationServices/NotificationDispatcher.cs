using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Notifications;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Domain.Entities.Notification;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class NotificationDispatcher(
    IServiceProvider sp,
    TimeProvider time,
    IUnitOfWork uow,
    ISmsSender smsSender,
    IEmailSender emailSender,
    IPushSender pushSender,
    ILogger<NotificationDispatcher> logger)
    : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);
    private const int BatchSize = 25;

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
            using var scope = sp.CreateScope();
            var repo = uow.GetEntityRepository<Notification>();

            var handlers = BuildHandlers();

            var batch = await FetchPendingBatch(repo, ct);
            foreach (var n in batch)
            {
                await ProcessOne(n, handlers, ct);
            }

            if (batch.Count > 0)
                await uow.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Notification dispatcher cycle failed");
        }
    }

    private static async Task<List<Notification>> FetchPendingBatch(
        IGenericRepository<Notification> repo,
        CancellationToken ct)
    {
        return await repo.DbSet
            .Where(n => n.Status == NotificationStatus.Pending)
            .OrderBy(n => n.CreatedDate)
            .Take(BatchSize)
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
                MarkFailed(n, "UNSUPPORTED_NOTIFICATION_CHANNEL");
                return;
            }

            var result = await handler(n, ct);
            ApplyResult(n, result);
        }
        catch (Exception ex)
        {
            n.MarkFailed(ex.Message);
            logger.LogError(ex, "Notification {Id} failed", n.Id);
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

    private static void MarkFailed(Notification n, string errorCode)
        => n.MarkFailed(errorCode);

    private IReadOnlyDictionary<NotificationChannel, Func<Notification, CancellationToken, Task<NotificationResponse>>> BuildHandlers()
    {
        return new Dictionary<NotificationChannel, Func<Notification, CancellationToken, Task<NotificationResponse>>>
        {
            [NotificationChannel.Email] = async (n, ct) =>
            {
                return await emailSender.SendAsync(n, ct);
            },

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
