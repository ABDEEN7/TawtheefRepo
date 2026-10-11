using Microsoft.Extensions.Options;
using Tawtheef.Domain.Configurations.Settings;

namespace Recruitment.API.Realtime;

public sealed class CandidateExamConnectionRenewalWorker(
    CandidateExamConnectionRegistry connectionRegistry,
    IServiceScopeFactory scopeFactory,
    IOptions<CandidateExamRealtimeSettings> options,
    ILogger<CandidateExamConnectionRenewalWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(
            TimeSpan.FromSeconds(options.Value.ConnectionRenewalSeconds));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var connectionIds = connectionRegistry.SnapshotConnectionIds();
            if (connectionIds.Count == 0)
                continue;

            try
            {
                using var scope = scopeFactory.CreateScope();
                var connectionService =
                    scope.ServiceProvider.GetRequiredService<CandidateExamConnectionService>();
                await connectionService.RenewAsync(connectionIds, stoppingToken);
            }
            catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning(exception, "Candidate exam connection renewal cycle failed");
            }
        }
    }
}
