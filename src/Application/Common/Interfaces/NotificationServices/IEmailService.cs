
using System.Threading.Tasks;

namespace Tawtheef.Application.Common.Interfaces.NotificationServices;

public interface IEmailService
{
    //Account Process
    Task SendConfirmationLinkEmailAsync(string email, string? recipientName, string link);
    Task SendEmailVerificationEmailSuccessAsync(string email, string userName);
    Task SendResetPassword(string email, string link);
}
