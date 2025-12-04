using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Tawtheef.Application.Common.Behaviours;

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
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            RegisterMapster(services);
            RegisterMediator(services);
            RegisterValidators(services);

            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            // Expose a time provider so services can rely on a testable time source.
            services.AddSingleton(TimeProvider.System);

            services.AddScoped<Common.Services.IProfileReviewService, Common.Services.ProfileReviewService>();
            services.AddScoped<Common.Services.IProfileStepValidationService, Common.Services.ProfileStepValidationService>();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        }

        #region Private registrations 
        private static void RegisterMapster(IServiceCollection services)
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(config);
            services.AddSingleton<IMapper>(sp => new ServiceMapper(sp, config));
        }

        private static void RegisterMediator(IServiceCollection services)
        {
            // Registers MediatR handlers from the current assembly.
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        }

        private static void RegisterValidators(IServiceCollection services)
        {
            // Registers FluentValidation validators from the current assembly.
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }

        #endregion
    }
}
