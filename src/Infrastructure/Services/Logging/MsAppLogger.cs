using Microsoft.Extensions.Logging;
using Tawtheef.Application.Common.Interfaces.Logging;

namespace Tawtheef.Infrastructure.Services.Logging;

public sealed class MsAppLogger : IAppLogger
{
    private readonly ILogger _logger;
    private readonly ILoggerFactory _factory;
    private readonly string _categoryName;

    public MsAppLogger(ILoggerFactory factory, string? categoryName = null)
    {
        _factory = factory;
        _categoryName = string.IsNullOrWhiteSpace(categoryName) ? "App" : categoryName;
        _logger = factory.CreateLogger(_categoryName);
    }

    public IAppLogger ForContext(Type type) =>
        new MsAppLogger(_factory, type.FullName ?? type.Name);

    public IAppLogger ForContext(string contextName) =>
        new MsAppLogger(_factory, contextName);

    public void Write(AppLogLevel level, string messageTemplate, params object?[] propertyValues) =>
        _logger.Log(MapLevel(level), messageTemplate, propertyValues);

    public void Write(
        Exception exception,
        AppLogLevel level,
        string messageTemplate,
        params object?[] propertyValues) =>
        _logger.Log(MapLevel(level), exception, messageTemplate, propertyValues);

    public void Verbose(string messageTemplate, params object?[] propertyValues) =>
        _logger.Log(LogLevel.Trace, messageTemplate, propertyValues);

    public void Verbose(Exception exception, string messageTemplate, params object?[] propertyValues) =>
        _logger.Log(LogLevel.Trace, exception, messageTemplate, propertyValues);

    public void Debug(string messageTemplate, params object?[] propertyValues) =>
        _logger.Log(LogLevel.Debug, messageTemplate, propertyValues);

    public void Debug(Exception exception, string messageTemplate, params object?[] propertyValues) =>
        _logger.Log(LogLevel.Debug, exception, messageTemplate, propertyValues);

    public void Information(string messageTemplate, params object?[] propertyValues) =>
        _logger.Log(LogLevel.Information, messageTemplate, propertyValues);

    public void Information(Exception exception, string messageTemplate, params object?[] propertyValues) =>
        _logger.Log(LogLevel.Information, exception, messageTemplate, propertyValues);

    public void Warning(string messageTemplate, params object?[] propertyValues) =>
        _logger.Log(LogLevel.Warning, messageTemplate, propertyValues);

    public void Warning(Exception exception, string messageTemplate, params object?[] propertyValues) =>
        _logger.Log(LogLevel.Warning, exception, messageTemplate, propertyValues);

    public void Error(string messageTemplate, params object?[] propertyValues) =>
        _logger.Log(LogLevel.Error, messageTemplate, propertyValues);

    public void Error(Exception exception, string messageTemplate, params object?[] propertyValues) =>
        _logger.Log(LogLevel.Error, exception, messageTemplate, propertyValues);

    public void Fatal(string messageTemplate, params object?[] propertyValues) =>
        _logger.Log(LogLevel.Critical, messageTemplate, propertyValues);

    public void Fatal(Exception exception, string messageTemplate, params object?[] propertyValues) =>
        _logger.Log(LogLevel.Critical, exception, messageTemplate, propertyValues);

    private static LogLevel MapLevel(AppLogLevel level) =>
        level switch
        {
            AppLogLevel.Verbose => LogLevel.Trace,
            AppLogLevel.Debug => LogLevel.Debug,
            AppLogLevel.Information => LogLevel.Information,
            AppLogLevel.Warning => LogLevel.Warning,
            AppLogLevel.Error => LogLevel.Error,
            AppLogLevel.Fatal => LogLevel.Critical,
            _ => LogLevel.Information
        };
}
