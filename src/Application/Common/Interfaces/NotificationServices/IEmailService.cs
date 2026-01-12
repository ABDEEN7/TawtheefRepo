
namespace Tawtheef.Application.Common.Interfaces.NotificationServices;

public interface IEmailService
{
    Task SendTemplateAsync(
        string templateKey, string subject, List<string> to, object model,
        List<string>? cc = null, CancellationToken ct = default);
}
