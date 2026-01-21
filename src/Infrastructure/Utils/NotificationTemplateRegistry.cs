using System.Collections.Concurrent;
using System.Reflection;
using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Infrastructure.Utils;

public static class NotificationTemplateRegistry
{
    private static readonly ConcurrentDictionary<string, Type> _map = new(StringComparer.OrdinalIgnoreCase);

    public static void Register<TModel>(string templateKey) where TModel : class
        => _map[templateKey] = typeof(TModel);

    public static void RegisterFromAssemblies(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            RegisterFromAssembly(assembly);
        }
    }

    public static void RegisterFromAssembly(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract)
            {
                continue;
            }

            var templateAttribute = type.GetCustomAttribute<NotificationTemplateAttribute>();
            if (templateAttribute is null)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(templateAttribute.TemplateKey))
            {
                throw new InvalidOperationException(
                    $"Notification template key is missing for model '{type.FullName}'.");
            }

            _map[templateAttribute.TemplateKey] = type;
        }
    }

    public static bool TryGetModelType(string templateKey, out Type modelType)
        => _map.TryGetValue(templateKey, out modelType!);
}
