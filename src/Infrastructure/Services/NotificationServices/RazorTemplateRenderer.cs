using RazorLight;
using Tawtheef.Application.Common.Interfaces.NotificationServices;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class RazorTemplateRenderer : IEmailTemplateRenderer
{
    private readonly RazorLightEngine _engine;
    private readonly IEmailBranding _branding;
    private readonly Dictionary<string, string> _keyMap; // normalized -> actual

    public RazorTemplateRenderer(IEmailBranding branding)
    {
        _branding = branding;

        var asm = typeof(RazorTemplateRenderer).Assembly;

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

    private static string Normalize(string k) => k.Replace('\\', '/');

    private Task<string> RenderAsync<T>(string templateKey, string kind, T model)
    {
        if (string.IsNullOrWhiteSpace(templateKey))
            throw new ArgumentException("Template key is required.", nameof(templateKey));

        var logical = Normalize($"Templates/{templateKey}/{templateKey}.{kind}.cshtml");

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
}
