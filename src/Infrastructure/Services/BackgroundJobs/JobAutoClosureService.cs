using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
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

                    var jobsToClose = await jobRepository.GetJobsToAutoCloseAsync(DateTime.UtcNow);

                    foreach (var job in jobsToClose)
                    {
                        await mediator.Send(new ChangeJobStatusCommand(job.Id, JobStatusIds.Closed), stoppingToken);
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
