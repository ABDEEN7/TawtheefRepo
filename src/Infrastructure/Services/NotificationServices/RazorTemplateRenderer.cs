using RazorLight;
using Tawtheef.Application.Common.Interfaces.NotificationServices;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class RazorTemplateRenderer : IEmailTemplateRenderer
{
    private readonly RazorLightEngine _engine;
    private readonly IEmailBranding _branding;
    private readonly HashSet<string> _resourceNames;
    public RazorTemplateRenderer(IEmailBranding branding)
    {
        _branding = branding;
        var asm = typeof(RazorTemplateRenderer).Assembly;
        _resourceNames = asm.GetManifestResourceNames()
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        _engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(typeof(RazorTemplateRenderer))
            .SetOperatingAssembly(asm)
            .UseMemoryCachingProvider()
            .EnableDebugMode()
            .Build();
    }

    private async Task<string> RenderAsync<T>(string templateKey, string kind, T model)
    {
        var key = $"Templates/{templateKey}/{templateKey}.{kind}.cshtml";
        var normalizedResources = _resourceNames.ToDictionary(
            resource => resource.Replace('\\', '/'),
            resource => resource,
            StringComparer.OrdinalIgnoreCase);
        var resolvedKey = normalizedResources.TryGetValue(key, out var actualKey)
            ? actualKey
            : null;
        try
        {
            return await _engine.CompileRenderAsync(
                resolvedKey ?? key,
                new TemplateContext<T> { Branding = _branding, Model = model });
        }
        catch (TemplateNotFoundException ex)
        {
            var available = string.Join(", ", _resourceNames.OrderBy(name => name));
            throw new InvalidOperationException(
                $"Template '{key}' not found. Available templates: {available}.",
                ex);
        }
    }

    public Task<string> RenderHtmlAsync<T>(string templateKey, T model)
        => RenderAsync(templateKey, "html", model);

    public Task<string> RenderTextAsync<T>(string templateKey, T model)
        => RenderAsync(templateKey, "txt", model);
}
