using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Services.BackgroundJobs
{
    public class JobAutoClosureService(
        IServiceScopeFactory scopeFactory,
        IAppLogger logger) : BackgroundService
    {
        private readonly IAppLogger _logger = logger.ForContext(typeof(JobAutoClosureService));
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
                        await mediator.Send(new ChangeJobStatusCommand(job.Id, JobStatusIds.Closed), stoppingToken);
                    }

                    _logger.Information("Job auto-closure completed. {Count} jobs closed.", jobsToClose.Count);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error running JobAutoClosureService");
                }

                await timer.WaitForNextTickAsync(stoppingToken);
            }
        }
    }
}


