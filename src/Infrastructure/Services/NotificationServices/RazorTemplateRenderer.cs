using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RazorLight;
using Tawtheef.Application.Common.Interfaces.NotificationServices;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class RazorTemplateRenderer : IEmailTemplateRenderer
{
    private readonly RazorLightEngine _engine;
    private readonly IEmailBranding _branding;
    private readonly Dictionary<string, string> _manifest; // normalized -> real

    public RazorTemplateRenderer(IEmailBranding branding)
    {
        _branding = branding;

        var asm = typeof(RazorTemplateRenderer).Assembly;

        // Build a lookup that ignores slash direction
        _manifest = asm.GetManifestResourceNames()
            .ToDictionary(n => n.Replace('\\','/'),
                n => n,
                StringComparer.OrdinalIgnoreCase);

        _engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(asm)
            .SetOperatingAssembly(asm)
            .UseMemoryCachingProvider()
            .Build();
    }


    private static string Desired(string name, string kind)
        => $"Templates/{name}/{name}.{kind}.cshtml"; // forward-slash logical key

    private string Resolve(string logicalKey)
    {
        var k = logicalKey.Replace('\\','/');
        return _manifest.TryGetValue(k, out var real) ? real : logicalKey;
    }

    public async Task<string> RenderHtmlAsync<T>(string templateKey, T model)
        => await _engine.CompileRenderAsync(
                Resolve(Desired(templateKey, "html")),
                new TemplateContext<T> { Branding = _branding, Model = model });

    public async Task<string> RenderTextAsync<T>(string templateKey, T model)
        => await _engine.CompileRenderAsync(
                Resolve(Desired(templateKey, "txt")),
                new TemplateContext<T> { Branding = _branding, Model = model });
}
