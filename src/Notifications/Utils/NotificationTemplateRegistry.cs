using System.Collections.Concurrent;
using System.Reflection;
using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Utils;


public static class NotificationTemplateRegistry
{
    private static readonly ConcurrentDictionary<Type, string> ModelToTemplateKey = new();
    private static readonly ConcurrentDictionary<string, Type> TemplateKeyToModel = new(StringComparer.Ordinal);

    public static void Register<TModel>(string templateKey)
        => Register(typeof(TModel), templateKey);

    public static void Register(Type modelType, string templateKey)
    {
        if (modelType is null) throw new ArgumentNullException(nameof(modelType));
        if (string.IsNullOrWhiteSpace(templateKey)) throw new ArgumentException("Template key is required.", nameof(templateKey));

        // Enforce one-to-one mapping.
        if (ModelToTemplateKey.TryGetValue(modelType, out var existingKey) && existingKey != templateKey)
            throw new InvalidOperationException($"Model '{modelType.FullName}' is already registered with key '{existingKey}'.");

        if (TemplateKeyToModel.TryGetValue(templateKey, out var existingType) && existingType != modelType)
            throw new InvalidOperationException($"Template key '{templateKey}' is already registered for model '{existingType.FullName}'.");

        ModelToTemplateKey[modelType] = templateKey;
        TemplateKeyToModel[templateKey] = modelType;
    }

    public static string GetTemplateKeyFor<TModel>()
        => GetTemplateKeyFor(typeof(TModel));

    public static string GetTemplateKeyFor(Type modelType)
    {
        if (ModelToTemplateKey.TryGetValue(modelType, out var key))
            return key;

        throw new KeyNotFoundException($"No template registered for model '{modelType.FullName}'.");
    }

    public static Type GetModelTypeFor(string templateKey)
    {
        if (TemplateKeyToModel.TryGetValue(templateKey, out var modelType))
            return modelType;

        throw new KeyNotFoundException($"No model registered for template key '{templateKey}'.");
    }

    public static void AutoRegisterFrom(params Assembly[] assemblies)
    {
        if (assemblies is null || assemblies.Length == 0)
            throw new ArgumentException("At least one assembly is required.", nameof(assemblies));

        foreach (var assembly in assemblies.Distinct())
        {
            var candidates = assembly
                .GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false })
                .Select(t => new { Type = t, Attr = t.GetCustomAttribute<NotificationTemplateAttribute>() })
                .Where(x => x.Attr is not null);

            foreach (var item in candidates)
                Register(item.Type, item.Attr!.TemplateKey);
        }
    }
}
