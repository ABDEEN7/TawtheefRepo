using Cortex.Mediator.DependencyInjection;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Recruitment
{
     /// <summary>
    /// Extension methods to register Application layer services (Mapster, Mediator, Validators, etc).
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApplicationRecruitment(IConfiguration config)
            {
                services.RegisterMapster();
                services.RegisterMediator(config);
                return services;
            }

            private void RegisterMapster()
            {
                var config = TypeAdapterConfig.GlobalSettings;
                config.Scan(typeof(RecruitmentAssemblyMarker).Assembly);
            }

            private void RegisterMediator(IConfiguration configuration)
            {
                services.AddCortexMediator(
                    configuration: configuration,
                    handlerAssemblyMarkerTypes:
                    [
                        typeof(RecruitmentAssemblyMarker)
                    ],
                    configure: o => o.AddDefaultBehaviors()
                );
            }
        }
    }
}
