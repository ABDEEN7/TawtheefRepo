using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Notification;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class NotificationDispatcher(
    IServiceProvider sp,
    TimeProvider time,
    ILogger<NotificationDispatcher> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = sp.CreateScope();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var email = scope.ServiceProvider.GetService<IEmailSender>();
                var sms   = scope.ServiceProvider.GetService<ISmsSender>();
                var push  = scope.ServiceProvider.GetService<IPushSender>();

                // Fetch small batch of pending
                var repo = uow.GetEntityRepository<Notification>();
                var batch = await repo.DbSet
                    .Where(n => n.Status == NotificationStatus.Pending)
                    .OrderBy(n => n.CreatedDate)
                    .Take(25)
                    .ToListAsync(stoppingToken);
                foreach (var n in batch)
                {
                    var result = (ok: false, providerId: (string?)null, error: (IReadOnlyList<IError>?)null);

                    try
                    {
                        switch (n.Channel)
                        {
                            case NotificationChannel.Email:
                                {
                                    if (email is null)
                                    {
                                        result = (false, null,
                                            new List<IError> { new Error("EMAIL_SENDER_NOT_CONFIGURED") });
                                        break;
                                    }

                                    result = await email.SendAsync(
                                        n.ToAddress!,
                                        n.Subject ?? string.Empty,
                                        n.Body!,
                                        stoppingToken
                                    );
                                    break;
                                }

                            case NotificationChannel.Sms:
                                {
                                    if (sms is null)
                                    {
                                        result = (false, null,
                                            new List<IError> { new Error("SMS_SENDER_NOT_CONFIGURED") });
                                        break;
                                    }

                                    result = await sms.SendAsync(
                                        n.ToAddress!,
                                        n.Body!,
                                        stoppingToken
                                    );
                                    break;
                                }

                            case NotificationChannel.Push:
                                {
                                    if (n.UserId is null)
                                    {
                                        result = (false, null, new List<IError> { new Error("USER_ID_REQUIRED") });
                                        break;
                                    }

                                    if (push is null)
                                    {
                                        result = (false, null,
                                            new List<IError> { new Error("PUSH_SENDER_NOT_CONFIGURED") });
                                        break;
                                    }

                                    result = await push.SendAsync(
                                        n.UserId.Value,
                                        n.Subject ?? string.Empty,
                                        n.Body!,
                                        stoppingToken
                                    );
                                    break;
                                }

                            case NotificationChannel.InApp:
                                result = (true, null, null);
                                break;
                        }

                        if (result.ok)
                        {
                            n.MarkSent(result.providerId, time.GetUtcNow().DateTime);
                        }
                        else
                        {
                            var errors = result.error?.Select(e => e.Message).ToList() ?? new List<string>();
                            n.MarkFailed(errors.Any()
                                ? string.Join(", ", errors)
                                : "UNKNOWN_ERROR");
                        }
                    }
                    catch (Exception ex)
                    {
                        n.MarkFailed(ex.Message);
                        logger.LogError(ex, "Notification {Id} failed", n.Id);
                    }
                }
                if (batch.Count > 0) await uow.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Notification dispatcher cycle failed");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
