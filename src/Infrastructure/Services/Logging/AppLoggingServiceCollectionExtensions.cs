using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Tawtheef.Application.Common.Interfaces.Logging;

namespace Tawtheef.Infrastructure.Services.Logging;


public enum AppLoggingProvider
{
    Serilog,
    Microsoft
}

public static class AppLoggingServiceCollectionExtensions
{
    public static IServiceCollection AddAppLogging(
        this IServiceCollection services,
        AppLoggingProvider provider = AppLoggingProvider.Serilog)
    {
        return provider switch
        {
            AppLoggingProvider.Serilog => services.AddSingleton<IAppLogger>(_ => new SerilogAppLogger(Log.Logger)),
            AppLoggingProvider.Microsoft => services.AddSingleton<IAppLogger>(sp =>
                new MsAppLogger(sp.GetRequiredService<ILoggerFactory>())),
            _ => services.AddSingleton<IAppLogger>(_ => new SerilogAppLogger(Log.Logger))
        };
    }
}
