using Application.Operation.Features.Employee.Job.Commands;
using Cortex.Mediator;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Services.BackgroundJobs
{
    public class JobAutoClosureService(
        IServiceScopeFactory scopeFactory,
        ILogger<JobAutoClosureService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromHours(1));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();

                    var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var jobsToClose = await jobRepository.GetJobsToAutoCloseAsync(DateTimeOffset.UtcNow);

                    foreach (var job in jobsToClose)
                    {
                        await mediator.SendCommandAsync<ChangeJobStatusCommand, IResult<Unit>>(new ChangeJobStatusCommand(job.Id, JobStatusIds.Closed), stoppingToken);
                    }

                    logger.LogInformation("Job auto-closure completed. {Count} jobs closed.", jobsToClose.Count);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error running JobAutoClosureService");
                }

                await timer.WaitForNextTickAsync(stoppingToken);
            }
        }
    }
}
