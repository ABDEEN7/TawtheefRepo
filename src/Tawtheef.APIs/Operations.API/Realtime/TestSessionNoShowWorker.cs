using Application.Operation.Features.Employee.TestSlots.Services;
using Microsoft.Extensions.Options;
using Tawtheef.Domain.Configurations.Settings;

namespace Operations.API.Realtime;

public sealed class TestSessionNoShowWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<TestSlotSettings> options,
    ILogger<TestSessionNoShowWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var timer =
            new PeriodicTimer(
                TimeSpan.FromSeconds(
                    Math.Max(
                        5,
                        options.Value.NoShowSweepSeconds)));

        while (await timer.WaitForNextTickAsync(
                   stoppingToken))
        {
            IReadOnlyList<
                DueTestSessionNoShowCandidate> dueCandidates;

            try
            {
                using var scope =
                    scopeFactory.CreateScope();

                var service =
                    scope.ServiceProvider
                        .GetRequiredService<
                            TestSessionNoShowService>();

                dueCandidates =
                    await service.GetDueAsync(
                        100,
                        stoppingToken);
            }
            catch (Exception exception)
                when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(
                    exception,
                    "Test session no-show discovery failed");

                continue;
            }

            foreach (var dueCandidate in dueCandidates)
            {
                try
                {
                    using var scope =
                        scopeFactory.CreateScope();

                    var service =
                        scope.ServiceProvider
                            .GetRequiredService<
                                TestSessionNoShowService>();

                    var result =
                        await service.ProcessAsync(
                            dueCandidate,
                            stoppingToken);

                    if (result.IsFailed)
                    {
                        logger.LogError(
                            "Test session no-show processing failed " +
                            "for candidate {CandidateId}, slot {TestSlotId}: {Errors}",
                            dueCandidate.CandidateId,
                            dueCandidate.TestSlotId,
                            string.Join(
                                ", ",
                                result.Errors.Select(
                                    error => error.Message)));
                    }
                }
                catch (Exception exception)
                    when (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogError(
                        exception,
                        "Test session no-show processing failed " +
                        "for candidate {CandidateId}, slot {TestSlotId}",
                        dueCandidate.CandidateId,
                        dueCandidate.TestSlotId);
                }
            }
        }
    }
}
