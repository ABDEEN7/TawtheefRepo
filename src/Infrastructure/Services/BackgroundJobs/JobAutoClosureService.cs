using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Infrastructure.Services.BackgroundJobs;

public class JobAutoClosureService(
    IServiceScopeFactory scopeFactory,
    IAppLogger logger) : BackgroundService
{
    private readonly IAppLogger _logger = logger.ForContext(typeof(JobAutoClosureService));

    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);
    private const int MaxDegreeOfParallelism = 5; // Number of jobs processed concurrently
    private const int BatchSize = 50; // Number of jobs to fetch per batch

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessJobsInBatchesAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unhandled error in JobAutoClosureService loop");
            }

            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task ProcessJobsInBatchesAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            List<Job> jobsToClose;

            // Fetch a batch of jobs
            using (var scope = scopeFactory.CreateScope())
            {
                var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
                var now = DateTimeOffset.UtcNow;

                jobsToClose = await jobRepository
                    .GetJobsToAutoCloseBatchAsync(now, BatchSize); // Materialize to avoid MARS issues
            }

            if (jobsToClose.Count == 0)
            {
                _logger.Information("No more jobs to close in this cycle.");
                break;
            }

            _logger.Information("Processing batch of {Count} jobs.", jobsToClose.Count);
            // small delay to avoid tight loop
            await Task.Delay(TimeSpan.FromSeconds(15), ct);
            using var semaphore = new SemaphoreSlim(MaxDegreeOfParallelism);

            var tasks = jobsToClose.Select(async job =>
            {
                await semaphore.WaitAsync(ct).ConfigureAwait(false);

                try
                {
                    using var jobScope = scopeFactory.CreateScope();
                    var mediator = jobScope.ServiceProvider.GetRequiredService<IMediator>();

                    var result = await mediator.Send(new ChangeJobStatusCommand(job.Id, JobStatusIds.Closed), ct);
                    if(result.IsFailed)
                        _logger.Error(result.Errors, "Failed to close job {JobId}", job.Id);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to close job {JobId}", job.Id);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);

            _logger.Information("Completed batch of {Count} jobs.", jobsToClose.Count);
        }
    }
}
