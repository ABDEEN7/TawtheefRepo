using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
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

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

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

    private Task<string> RenderAsync<T>(string templateKey, string kind, string language, T model)
    {
        if (string.IsNullOrWhiteSpace(templateKey))
            throw new ArgumentException("Template key is required.", nameof(templateKey));

        // Try language-specific template first: Template.ar.html.cshtml
        var logicalLang = Normalize($"Tawtheef/Notifications/Templates/{templateKey}/{templateKey}.{language}.{kind}.cshtml");
        
        if (!_keyMap.TryGetValue(logicalLang, out var actual))
        {
            // Fallback to default template: Template.html.cshtml
            var logicalDefault = Normalize($"Tawtheef/Notifications/Templates/{templateKey}/{templateKey}.{kind}.cshtml");
            if (!_keyMap.TryGetValue(logicalDefault, out actual))
            {
                var available = string.Join("\n", _keyMap.Keys.OrderBy(x => x));
                throw new InvalidOperationException(
                    $"Template '{logicalDefault}' (or language '{language}') not found. Embedded templates:\n{available}");
            }
        }

        return _engine.CompileRenderAsync(
            actual,
            new TemplateContext<T> { Branding = _branding, Model = model, Language = language });
    }

    public Task<string> RenderHtmlAsync<T>(string templateKey, T model)
        => RenderAsync(templateKey, "html", "ar", model);

    public Task<string> RenderTextAsync<T>(string templateKey, T model)
        => RenderAsync(templateKey, "txt", "ar", model);

    public async Task<string> RenderHtmlAsync(string templateKey, string payloadJson, string language = "ar")
    {
        var model = DeserializePayload(templateKey, payloadJson);
        var method = GetGenericRenderMethod(nameof(RenderHtmlAsync), model.GetType());
        return await (Task<string>)method.Invoke(this, [templateKey, model, language])!;
    }

    public async Task<string> RenderTextAsync(string templateKey, string payloadJson, string language = "ar")
    {
        var model = DeserializePayload(templateKey, payloadJson);
        var method = GetGenericRenderMethod(nameof(RenderTextAsync), model.GetType());
        return await (Task<string>)method.Invoke(this, [templateKey, model, language])!;
    }

    public Task<string> RenderHtmlAsync<T>(string templateKey, T model, string language)
        => RenderAsync(templateKey, "html", language, model);

    public Task<string> RenderTextAsync<T>(string templateKey, T model, string language)
        => RenderAsync(templateKey, "txt", language, model);
    
    public string GetDefaultSubject(string templateKey, string language = "ar")
    {
        if (_modelTypeMap.TryGetValue(templateKey, out var type))
        {
            var attr = type.GetCustomAttribute<NotificationTemplateAttribute>();
            if (attr == null) return string.Empty;
            return language.Equals("ar", StringComparison.OrdinalIgnoreCase) 
                ? attr.SubjectAr 
                : attr.SubjectEn;
        }
        return string.Empty;
    }

    private MethodInfo GetGenericRenderMethod(string name, Type modelType)
    {
        return typeof(RazorTemplateRenderer)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .First(m => m.Name == name && m.IsGenericMethod && m.GetParameters().Length == 3)
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
                return JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson, _jsonOptions) ?? new object();
        }

        return JsonSerializer.Deserialize(payloadJson, type, _jsonOptions) ?? new object();
    }
}
