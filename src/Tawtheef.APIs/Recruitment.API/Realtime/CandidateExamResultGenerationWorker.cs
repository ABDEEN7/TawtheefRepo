using Microsoft.Extensions.Options;
using Tawtheef.Domain.Configurations.Settings;

namespace Recruitment.API.Realtime;

public sealed class CandidateExamResultGenerationWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<CandidateExamRealtimeSettings> options,
    ILogger<CandidateExamResultGenerationWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(
            TimeSpan.FromSeconds(
                Math.Max(
                    5,
                    options.Value
                        .ResultGenerationIntervalSeconds)));

        while (await timer.WaitForNextTickAsync(
                   stoppingToken))
        {
            IReadOnlyList<Guid> examIds;

            try
            {
                using var scope =
                    scopeFactory.CreateScope();

                var service = scope.ServiceProvider
                    .GetRequiredService<
                        CandidateExamResultGenerationService>();

                examIds =
                    await service.GetReadyExamIdsAsync(
                        20,
                        stoppingToken);
            }
            catch (Exception exception)
                when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(
                    exception,
                    "Candidate exam result processing discovery failed");

                continue;
            }

            foreach (var examId in examIds)
            {
                try
                {
                    // Fresh DbContext per exam.
                    using var scope =
                        scopeFactory.CreateScope();

                    var service = scope.ServiceProvider
                        .GetRequiredService<
                            CandidateExamResultGenerationService>();

                    await service.ProcessReadyResultsAsync(
                        examId,
                        stoppingToken);
                }
                catch (Exception exception)
                    when (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogError(
                        exception,
                        "Candidate exam result processing failed for exam {ExamId}",
                        examId);
                }
            }
        }
    }
}
