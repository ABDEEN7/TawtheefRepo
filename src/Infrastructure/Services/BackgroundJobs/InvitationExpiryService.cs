using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Infrastructure.Services.BackgroundJobs;

public sealed class InvitationExpiryService(
    IServiceScopeFactory scopeFactory,
    IAppLogger logger) : BackgroundService
{
    private readonly IAppLogger _logger = logger.ForContext(typeof(InvitationExpiryService));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DelayUntilNextMidnight(stoppingToken);
                await ExpireInvitationsAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unhandled error while expiring invitations");
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }

    private static async Task DelayUntilNextMidnight(CancellationToken ct)
    {
        var now = DateTimeOffset.Now;
        var nextMidnight = new DateTimeOffset(now.Date.AddDays(1), now.Offset);
        await Task.Delay(nextMidnight - now, ct);
    }

    private async Task ExpireInvitationsAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var today = DateOnly.FromDateTime(DateTime.Now);

        var expirableStatuses = new[]
        {
            InvitationStatusIds.NewInvitation,
            InvitationStatusIds.Read
        };

        var updated = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .Where(i => expirableStatuses.Contains(i.InvitationStatusId) && i.ExpiresOn < today)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(i => i.InvitationStatusId, InvitationStatusIds.Expired)
                .SetProperty(i => i.UpdatedDate, DateTime.UtcNow), ct);

        _logger.Information("Expired {Count} invitations.", updated);
    }
}
