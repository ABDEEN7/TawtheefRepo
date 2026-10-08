using Microsoft.Extensions.Options;
using Tawtheef.Domain.Configurations.Settings;

namespace Recruitment.API.Realtime;

public sealed class CandidateExamInterruptionWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<CandidateExamRealtimeSettings> options,
    ILogger<CandidateExamInterruptionWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(
            TimeSpan.FromSeconds(Math.Max(5, options.Value.ConnectionRenewalSeconds)));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            IReadOnlyList<Guid> dueAttemptIds;
            try
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<CandidateExamInterruptionService>();
                dueAttemptIds = await service.GetDueAttemptIdsAsync(50, stoppingToken);
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Candidate exam interruption discovery failed");
                continue;
            }

            foreach (var attemptId in dueAttemptIds)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<CandidateExamInterruptionService>();
                    if (await service.TryInterruptAsync(attemptId, stoppingToken))
                        logger.LogInformation(
                            "Candidate exam attempt {AttemptId} transitioned to Interrupted", attemptId);
                }
                catch (Exception exception)
                {
                    logger.LogWarning(exception,
                        "Candidate exam interruption evaluation failed for attempt {AttemptId}", attemptId);
                }
            }
        }
    }
}
