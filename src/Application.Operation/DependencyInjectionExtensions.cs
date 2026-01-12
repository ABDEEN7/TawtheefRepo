using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;
using Cortex.Mediator.DependencyInjection;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tawtheef.Application;

namespace Application.Operation
{
     /// <summary>
    /// Extension methods to register Application layer services (Mapster, Mediator, Validators, etc).
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApplicationOperation(IConfiguration config)
            {
                services.RegisterMapster();
                services.RegisterMediator(config);
                services.RegisterRepositories();
                return services;
            }

            private void RegisterMapster()
            {
                var config = TypeAdapterConfig.GlobalSettings;
                config.Scan(typeof(OperationAssemblyMarker).Assembly);
            }

            private void RegisterMediator(IConfiguration configuration)
            {
                services.AddCortexMediator(
                    configuration: configuration,
                    handlerAssemblyMarkerTypes:
                    [
                        typeof(ApplicationAssemblyMarker),
                        typeof(OperationAssemblyMarker)
                    ],
                    configure: o => o.AddDefaultBehaviors()
                );
            }
            
            private void RegisterRepositories()
            {
                services.AddScoped<IJobTargetCandidateCalculatorService, JobTargetCandidateCalculatorService>();
                services.AddScoped<IJobRequirementsService, JobRequirementsService>();
                services.AddScoped<IJobCandidatesQueryBuilderService, JobCandidatesQueryBuilderService>();
            }
        }
    }
}
