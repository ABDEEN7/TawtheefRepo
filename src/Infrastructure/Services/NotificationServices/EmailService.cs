using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Notifications.Interfaces;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public partial class EmailService(
    IEmailTemplateRenderer renderer,
    IUnitOfWork uow,
    IAppLogger log) : IEmailService
{
    private async Task EnqueueUsingTemplate<T>(
        string templateKey, string subject,
        List<string> to, T model, List<string>? cc = null,
        CancellationToken ct = default, string? idempotencyKey = null)
    {
        // 1. Check for duplicate if idempotencyKey provided
        if (!string.IsNullOrEmpty(idempotencyKey))
        {
            var exists = await uow.GetEntityRepository<Notification>()
                .DbSet.AnyAsync(n => n.IdempotencyKey == idempotencyKey, ct);
            if (exists)
            {
                log.Warning("Duplicate email notification blocked by idempotency key: {Key}", idempotencyKey);
                return;
            }
        }

        // 2. Render template
        var html = await renderer.RenderHtmlAsync(templateKey, model);
        var text = await renderer.RenderTextAsync(templateKey, model);
        var plainText = string.IsNullOrWhiteSpace(text) ? HtmlAgilityPackRegex().Replace(html, string.Empty) : text;

        // 3. Create Notification entity
        var payloadJson = JsonSerializer.Serialize(model);
        var toStr = string.Join(';', to);
        var ccStr = cc != null ? string.Join(';', cc) : null;

        var notification = Notification.Create(
            NotificationChannel.Email, templateKey, null,
            toStr, subject, html, plainText, payloadJson,
            idempotencyKey);

        // 4. Save to DB
        await uow.GetEntityRepository<Notification>().AddAsync(notification, ct);
        await uow.SaveChangesAsync(ct);
        
        log.Information("Email notification persisted for background delivery; to={To}; subject={Subject}; idempotency={Key}", 
            toStr, subject, idempotencyKey);
    }
    
    public Task SendTemplateAsync<T>(
        string templateKey, string subject, List<string> to, T model,
        List<string>? cc = null, CancellationToken ct = default, 
        string? idempotencyKey = null)
        => EnqueueUsingTemplate(templateKey, subject, to, model, cc, ct, idempotencyKey);

    [GeneratedRegex("<.*?>")]
    private static partial Regex HtmlAgilityPackRegex();
}
