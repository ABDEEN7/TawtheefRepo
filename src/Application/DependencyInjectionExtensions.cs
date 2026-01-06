using System.Linq.Expressions;
using System.Reflection;
using Cortex.Mediator.DependencyInjection;
using FluentValidation;
using Mapster;
using MapsterMapper;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Domain;

namespace Tawtheef.Application
{
    /// <summary>
    /// Extension methods to register Application layer services (AutoMapper, MediatR, Validators, etc).
    /// Kept minimal and easy to extend (pipeline behaviors, logging, etc).
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Registers application-level services into DI.
        /// </summary>
        public static void AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterMapster(services);
            RegisterMediator(services, configuration);
            RegisterValidators(services);

            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            // Expose a time provider so services can rely on a testable time source.
            services.AddSingleton(TimeProvider.System);
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        }

        #region Private registrations 
        private static void RegisterMapster(IServiceCollection services)
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());
            config.Scan(typeof(ResourceMapper).Assembly);
            services.AddSingleton(config);
            services.AddScoped<IMapper>(sp => new ServiceMapper(sp, config));
            #if DEBUG
            TypeAdapterConfig.GlobalSettings.Compiler = exp => exp.CompileWithDebugInfo();
            #endif
        }

        private static void RegisterMediator(IServiceCollection services, IConfiguration configuration)
        {
            // Registers MediatR handlers from the current assembly.
            // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            
            services.AddCortexMediator(
                configuration: configuration,
                handlerAssemblyMarkerTypes: [
                    typeof(ApplicationAssemblyMarker),
                    typeof(DomainAssemblyMarker)
                ],
                configure: options =>
                {
                    // This enables built-in logging, validation, and transaction behaviors
                    options.AddDefaultBehaviors();
                }
            );
        }

        private static void RegisterValidators(IServiceCollection services)
        {
            // Registers FluentValidation validators from the current assembly.
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }

        #endregion
    }
}
