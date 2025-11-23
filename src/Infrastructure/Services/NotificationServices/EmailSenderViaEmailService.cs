using FluentResults;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Interfaces.Services;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class EmailSenderViaEmailService(IEmailService emailService) : IEmailSender
{
    public async Task<(bool ok, string? providerId, IReadOnlyList<IError>? error)>
        SendAsync(string to, string? subject, string bodyHtml, CancellationToken ct)
    {
        await emailService.SendHtmlAsync(
            subject ?? "Notification", [to], bodyHtml, null, ct);

        // The queue returns immediately; dispatcher/transport will actually deliver
        return (true, Guid.NewGuid().ToString("N"), null);
    }
}
