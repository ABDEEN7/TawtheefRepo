
namespace Tawtheef.Application.Common.Interfaces.NotificationServices;

public interface IEmailService
{
    //Account Process
    Task SendConfirmationLinkEmailAsync(string email, string? recipientName, string link);
    Task SendEmailVerificationEmailSuccessAsync(string email, string userName);
    Task SendResetPassword(string email, string link);
    Task SendTemplateAsync(
        string templateKey, string subject, List<string> to, object model,
        List<string>? cc = null, CancellationToken ct = default);

    Task SendHtmlAsync(
        string subject, List<string> to, string html,
        List<string>? cc = null, CancellationToken ct = default);
}
