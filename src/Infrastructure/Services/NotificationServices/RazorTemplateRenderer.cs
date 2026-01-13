using System.Linq;
using RazorLight;
using Tawtheef.Application.Common.Interfaces.NotificationServices;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class RazorTemplateRenderer : IEmailTemplateRenderer
{
    private readonly RazorLightEngine _engine;
    private readonly IEmailBranding _branding;
    private readonly string _root;
    private readonly HashSet<string> _resourceNames;
    public RazorTemplateRenderer(IEmailBranding branding)
    {
        _branding = branding;
        var asm = typeof(RazorTemplateRenderer).Assembly;
        _root = asm.GetName().Name!;
        _resourceNames = asm.GetManifestResourceNames()
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        _engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(typeof(RazorTemplateRenderer))
            .SetOperatingAssembly(asm)
            .UseMemoryCachingProvider()
            .EnableDebugMode()
            .Build();
    }

    private string Key(string name, string kind)
    {
        var suffix = $".Templates.{name}.{name}.{kind}.cshtml";
        var match = _resourceNames.FirstOrDefault(resource => resource.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
        return match ?? $"{_root}{suffix}";
    }

    public Task<string> RenderHtmlAsync<T>(string templateKey, T model)
        => _engine.CompileRenderAsync(
            Key(templateKey, "html"),
            new TemplateContext<T> { Branding = _branding, Model = model });

    public Task<string> RenderTextAsync<T>(string templateKey, T model)
        => _engine.CompileRenderAsync(
            Key(templateKey, "txt"),
            new TemplateContext<T> { Branding = _branding, Model = model });
}
