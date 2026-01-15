namespace Tawtheef.Application.Common.Interfaces.Services.Security;

public interface IVerificationService
{
    Task LogVerificationAttempt(string email);
    bool CanResendEmail(string email);
    int GetResendCooldown(string email);
}
