using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tawtheef.Application;

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
                services.AddApplicationLayer(config);
                services.RegisterMapster();
                services.RegisterMediator();
                return services;
            }

            private void RegisterMapster()
            {
                var config = TypeAdapterConfig.GlobalSettings;
                config.Scan(typeof(RecruitmentAssemblyMarker).Assembly);
            }

            private void RegisterMediator()
            {
                services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
                    cfg.RegisterServicesFromAssembly(typeof(RecruitmentAssemblyMarker).Assembly);
                    cfg.AddOpenBehavior(typeof(Tawtheef.Application.Common.Behaviours.ValidationBehaviour<,>));
                });
            }
        }
    }
}
