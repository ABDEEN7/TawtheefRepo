namespace Tawtheef.Application.Common.Interfaces.Logging;

public enum AppLogLevel
{
    Verbose,
    Debug,
    Information,
    Warning,
    Error,
    Fatal
}


public interface IAppLogger
{
    IAppLogger ForContext(Type contextName);
    IAppLogger ForContext(string contextName);

    void Write(AppLogLevel level, string messageTemplate, params object?[] propertyValues);
    void Write(Exception exception, AppLogLevel level, string messageTemplate, params object?[] propertyValues);

    void Verbose(string messageTemplate, params object?[] propertyValues);
    void Verbose(Exception exception, string messageTemplate, params object?[] propertyValues);

    void Debug(string messageTemplate, params object?[] propertyValues);
    void Debug(Exception exception, string messageTemplate, params object?[] propertyValues);

    void Information(string messageTemplate, params object?[] propertyValues);
    void Information(Exception exception, string messageTemplate, params object?[] propertyValues);

    void Warning(string messageTemplate, params object?[] propertyValues);
    void Warning(Exception exception, string messageTemplate, params object?[] propertyValues);

    void Error(string messageTemplate, params object?[] propertyValues);
    void Error(Exception exception, string messageTemplate, params object?[] propertyValues);

    void Fatal(string messageTemplate, params object?[] propertyValues);
    void Fatal(Exception exception, string messageTemplate, params object?[] propertyValues);
}
