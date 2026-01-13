using RazorLight;
using Tawtheef.Application.Common.Interfaces.NotificationServices;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class RazorTemplateRenderer : IEmailTemplateRenderer
{
    private readonly RazorLightEngine _engine;
    private readonly IEmailBranding _branding;
    private readonly string _root;
    public RazorTemplateRenderer(IEmailBranding branding)
    {
        _branding = branding;
        var asm = typeof(RazorTemplateRenderer).Assembly;
        _root = asm.GetName().Name!;
        
        _engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(typeof(RazorTemplateRenderer))
            .SetOperatingAssembly(asm)
            .UseMemoryCachingProvider()
            .EnableDebugMode()
            .Build();
    }

    private string Key(string name, string kind)
        => $"{_root}.Templates.{name}.{name}.{kind}.cshtml";

    public Task<string> RenderHtmlAsync<T>(string templateKey, T model)
        => _engine.CompileRenderAsync(
            Key(templateKey, "html"),
            new TemplateContext<T> { Branding = _branding, Model = model });

    public Task<string> RenderTextAsync<T>(string templateKey, T model)
        => _engine.CompileRenderAsync(
            Key(templateKey, "txt"),
            new TemplateContext<T> { Branding = _branding, Model = model });
}
