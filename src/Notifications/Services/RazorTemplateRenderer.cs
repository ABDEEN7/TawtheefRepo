using System.Reflection;
using System.Text.Json;
using RazorLight;
using Tawtheef.Notifications.Attributes;
using Tawtheef.Notifications.Context;
using Tawtheef.Notifications.Interfaces;

namespace Tawtheef.Notifications.Services;

public sealed class RazorTemplateRenderer : IEmailTemplateRenderer
{
    private readonly RazorLightEngine _engine;
    private readonly IEmailBranding _branding;
    private readonly Dictionary<string, string> _keyMap; // normalized -> actual

    public RazorTemplateRenderer(IEmailBranding branding)
    {
        _branding = branding;

        var asm = typeof(NotificationAssemblyMarker).Assembly;

        _keyMap = asm.GetManifestResourceNames()
            .ToDictionary(Normalize, k => k, StringComparer.OrdinalIgnoreCase);

        var builder = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(asm)
            .SetOperatingAssembly(asm)
            .UseMemoryCachingProvider();

        #if DEBUG
        builder = builder.EnableDebugMode();
        #endif

        _engine = builder.Build();
    }

    private static string Normalize(string k) => k.Replace('/', '.');

    private Task<string> RenderAsync<T>(string templateKey, string kind, T model)
    {
        if (string.IsNullOrWhiteSpace(templateKey))
            throw new ArgumentException("Template key is required.", nameof(templateKey));

        var logical = Normalize($"Tawtheef/Notifications/Templates/{templateKey}/{templateKey}.{kind}.cshtml");

        if (!_keyMap.TryGetValue(logical, out var actual))
        {
            var available = string.Join("\n", _keyMap.Keys.OrderBy(x => x));
            throw new InvalidOperationException(
                $"Template '{logical}' not found. Embedded templates:\n{available}");
        }

        return _engine.CompileRenderAsync(
            actual,
            new TemplateContext<T> { Branding = _branding, Model = model });
    }

    public Task<string> RenderHtmlAsync<T>(string templateKey, T model)
        => RenderAsync(templateKey, "html", model);

    public Task<string> RenderTextAsync<T>(string templateKey, T model)
        => RenderAsync(templateKey, "txt", model);

    public async Task<string> RenderHtmlAsync(string templateKey, string payloadJson)
    {
        var model = DeserializePayload(templateKey, payloadJson);
        var method = GetGenericRenderMethod(nameof(RenderHtmlAsync), model.GetType());
        return await (Task<string>)method.Invoke(this, [templateKey, model])!;
    }

    public async Task<string> RenderTextAsync(string templateKey, string payloadJson)
    {
        var model = DeserializePayload(templateKey, payloadJson);
        var method = GetGenericRenderMethod(nameof(RenderTextAsync), model.GetType());
        return await (Task<string>)method.Invoke(this, [templateKey, model])!;
    }

    private MethodInfo GetGenericRenderMethod(string name, Type modelType)
    {
        return typeof(RazorTemplateRenderer)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .First(m => m.Name == name && m.IsGenericMethod)
            .MakeGenericMethod(modelType);
    }

    private static readonly Dictionary<string, Type> _modelTypeMap = InitModelMap();

    private static Dictionary<string, Type> InitModelMap()
    {
        var map = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        var types = typeof(NotificationAssemblyMarker).Assembly.GetTypes();
        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<NotificationTemplateAttribute>();
            if (attr != null)
            {
                map[attr.TemplateKey] = type;
            }
        }
        return map;
    }

    private object DeserializePayload(string templateKey, string payloadJson)
    {
        if (string.IsNullOrWhiteSpace(payloadJson))
            return new object(); // or null, but templates might fail

        if (!_modelTypeMap.TryGetValue(templateKey, out var type))
        {
            // Fallback: try to find by name if not attributed
            // Tawtheef.Notifications.Templates.{templateKey}.{templateKey}Model
            var fallbackName = $"Tawtheef.Notifications.Templates.{templateKey}.{templateKey}Model";
            type = typeof(NotificationAssemblyMarker).Assembly.GetType(fallbackName);
            
            if (type == null)
                return JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson) ?? new object();
        }

        return JsonSerializer.Deserialize(payloadJson, type) ?? new object();
    }
}
