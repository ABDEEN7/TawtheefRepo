using Microsoft.Extensions.Options;
using Tawtheef.Domain.Configurations.Settings;

namespace Recruitment.API.Realtime;

public sealed class CandidateExamDeadlineWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<CandidateExamRealtimeSettings> options,
    ILogger<CandidateExamDeadlineWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(
            TimeSpan.FromSeconds(
                Math.Max(5, options.Value.ConnectionRenewalSeconds)));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            IReadOnlyList<DueCandidateExamPart> dueParts;

            try
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider
                    .GetRequiredService<CandidateExamDeadlineService>();

                dueParts = await service.GetDuePartsAsync(50, stoppingToken);
            }
            catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(
                    exception,
                    "Candidate exam deadline discovery failed");
                continue;
            }

            foreach (var duePart in dueParts)
            {
                try
                {
                    // Separate scope: a failure on one attempt cannot leave
                    // tracked entities affecting the next attempt.
                    using var scope = scopeFactory.CreateScope();
                    var service = scope.ServiceProvider
                        .GetRequiredService<CandidateExamDeadlineService>();

                    var result = await service.ProcessAsync(
                        duePart,
                        stoppingToken);

                    if (result.IsFailed)
                    {
                        logger.LogError(
                            "Candidate exam deadline processing failed for attempt {AttemptId}, part {PartId}: {Errors}",
                            duePart.AttemptId,
                            duePart.PartId,
                            string.Join(", ", result.Errors.Select(error => error.Message)));
                    }
                }
                catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogError(
                        exception,
                        "Candidate exam deadline processing failed for attempt {AttemptId}, part {PartId}",
                        duePart.AttemptId,
                        duePart.PartId);
                }
            }
        }
    }
}
