using System.Collections.Concurrent;

namespace Tawtheef.Infrastructure.Utils;

public static class NotificationTemplateRegistry
{
    private static readonly ConcurrentDictionary<string, Type> _map = new(StringComparer.OrdinalIgnoreCase);

    public static void Register<TModel>(string templateKey) where TModel : class
        => _map[templateKey] = typeof(TModel);

    public static bool TryGetModelType(string templateKey, out Type modelType)
        => _map.TryGetValue(templateKey, out modelType!);
}