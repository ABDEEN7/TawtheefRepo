using System.Collections.Generic;
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

    private IEnumerable<string> KeyCandidates(string name, string kind)
    {
        var suffix = $".Templates.{name}.{name}.{kind}.cshtml";
        var defaultKey = $"{_root}{suffix}";
        var relativeKey = $"Templates.{name}.{name}.{kind}.cshtml";
        var match = _resourceNames.FirstOrDefault(resource => resource.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));

        var candidates = new List<string> { defaultKey, relativeKey };
        if (!string.IsNullOrWhiteSpace(match))
        {
            candidates.Add(match);
            var withoutRoot = match.StartsWith($"{_root}.", StringComparison.OrdinalIgnoreCase)
                ? match[_root.Length + 1..]
                : match;
            candidates.Add(withoutRoot);
        }

        return candidates.Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private async Task<string> RenderAsync<T>(string templateKey, string kind, T model)
    {
        Exception? lastException = null;
        foreach (var candidate in KeyCandidates(templateKey, kind))
        {
            try
            {
                return await _engine.CompileRenderAsync(
                    candidate,
                    new TemplateContext<T> { Branding = _branding, Model = model });
            }
            catch (TemplateNotFoundException ex)
            {
                lastException = ex;
            }
        }

        throw lastException ?? new TemplateNotFoundException(templateKey);
    }

    public Task<string> RenderHtmlAsync<T>(string templateKey, T model)
        => RenderAsync(templateKey, "html", model);

    public Task<string> RenderTextAsync<T>(string templateKey, T model)
        => RenderAsync(templateKey, "txt", model);
}
