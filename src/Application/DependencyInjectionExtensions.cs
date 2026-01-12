using System.Linq.Expressions;
using System.Reflection;
using Cortex.Mediator.DependencyInjection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tawtheef.Domain.Configurations.Settings;

namespace Tawtheef.Application
{
     /// <summary>
    /// Extension methods to register Application layer services (Mapster, Mediator, Validators, etc).
    /// Split by App:Module (Recruitment / Operation / Both).
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Keep runtime settings available in DI (same shape as Infrastructure).
            services.AddOptions<AppRuntimeSettings>()
                .Bind(configuration.GetSection(AppRuntimeSettings.SectionName))
                .Validate(s => s.Module != 0, "App:Module is required. Allowed: Recruitment | Operation | Both.")
                .ValidateOnStart();

            // ===== Common (always) =====
            RegisterMapster(services);
            RegisterMediator(services, configuration);
            RegisterValidators(services);

            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddSingleton(TimeProvider.System);
        }

        #region Private registrations

        private static void RegisterMapster(IServiceCollection services)
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(ApplicationAssemblyMarker).Assembly);

            services.AddSingleton(config);
            services.AddScoped<IMapper>(sp => new ServiceMapper(sp, config));

#if DEBUG
            TypeAdapterConfig.GlobalSettings.Compiler = exp => exp.CompileWithDebugInfo();
#endif
        }

        private static void RegisterMediator(IServiceCollection services, IConfiguration configuration)
        {
            services.AddCortexMediator(
                    configuration: configuration,
                    handlerAssemblyMarkerTypes:
                    [
                        typeof(ApplicationAssemblyMarker)
                    ],
                    configure: o => o.AddDefaultBehaviors()
                );
        }

        private static void RegisterValidators(IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }

        #endregion
    }
}
