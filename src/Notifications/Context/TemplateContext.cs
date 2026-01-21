using Tawtheef.Notifications.Interfaces;

namespace Tawtheef.Notifications.Context;

public record TemplateContext<T>
{
    public required T Model { get; init; }
    public required IEmailBranding Branding { get; init; }
}
