using Serilog;
using Serilog.Events;
using Tawtheef.Application.Common.Interfaces.Logging;

namespace Tawtheef.Infrastructure.Services.Logging;


public sealed class SerilogAppLogger(ILogger logger) : IAppLogger
{
    public IAppLogger ForContext(Type type) => 
        new SerilogAppLogger(logger.ForContext(type));

    public IAppLogger ForContext(string contextName) =>
        new SerilogAppLogger(logger.ForContext("SourceContext", contextName));

    public void Write(AppLogLevel level, string messageTemplate, params object?[] propertyValues) =>
        logger.Write(MapLevel(level), messageTemplate, propertyValues);

    public void Write(
        Exception exception,
        AppLogLevel level,
        string messageTemplate,
        params object?[] propertyValues) =>
        logger.Write(MapLevel(level), exception, messageTemplate, propertyValues);

    public void Verbose(string messageTemplate, params object?[] propertyValues) =>
        logger.Verbose(messageTemplate, propertyValues);

    public void Verbose(Exception exception, string messageTemplate, params object?[] propertyValues) =>
        logger.Verbose(exception, messageTemplate, propertyValues);

    public void Debug(string messageTemplate, params object?[] propertyValues) =>
        logger.Debug(messageTemplate, propertyValues);

    public void Debug(Exception exception, string messageTemplate, params object?[] propertyValues) =>
        logger.Debug(exception, messageTemplate, propertyValues);

    public void Information(string messageTemplate, params object?[] propertyValues) =>
        logger.Information(messageTemplate, propertyValues);

    public void Information(Exception exception, string messageTemplate, params object?[] propertyValues) =>
        logger.Information(exception, messageTemplate, propertyValues);

    public void Warning(string messageTemplate, params object?[] propertyValues) =>
        logger.Warning(messageTemplate, propertyValues);

    public void Warning(Exception exception, string messageTemplate, params object?[] propertyValues) =>
        logger.Warning(exception, messageTemplate, propertyValues);

    public void Error(string messageTemplate, params object?[] propertyValues) =>
        logger.Error(messageTemplate, propertyValues);

    public void Error(Exception exception, string messageTemplate, params object?[] propertyValues) =>
        logger.Error(exception, messageTemplate, propertyValues);

    public void Fatal(string messageTemplate, params object?[] propertyValues) =>
        logger.Fatal(messageTemplate, propertyValues);

    public void Fatal(Exception exception, string messageTemplate, params object?[] propertyValues) =>
        logger.Fatal(exception, messageTemplate, propertyValues);

    private static LogEventLevel MapLevel(AppLogLevel level) =>
        level switch
        {
            AppLogLevel.Verbose => LogEventLevel.Verbose,
            AppLogLevel.Debug => LogEventLevel.Debug,
            AppLogLevel.Information => LogEventLevel.Information,
            AppLogLevel.Warning => LogEventLevel.Warning,
            AppLogLevel.Error => LogEventLevel.Error,
            AppLogLevel.Fatal => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
}
