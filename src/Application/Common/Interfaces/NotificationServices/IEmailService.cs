
namespace Tawtheef.Application.Common.Interfaces.NotificationServices;

public interface IEmailService
{
    Task SendTemplateAsync<T>(
        string templateKey, string subject, List<string> to, T model,
        List<string>? cc = null, CancellationToken ct = default, 
        string? idempotencyKey = null);
}
