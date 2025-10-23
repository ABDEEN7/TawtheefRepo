using Tawtheef.Application.Common.Interfaces.NotificationServices;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public record TemplateContext<T>
{
    public required T Model { get; init; }
    public required IEmailBranding Branding { get; init; }
}