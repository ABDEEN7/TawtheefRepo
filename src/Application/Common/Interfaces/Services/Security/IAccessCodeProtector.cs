namespace Tawtheef.Application.Common.Interfaces.Services.Security;

public interface IAccessCodeProtector
{
    string Protect(string accessCode);
    string Unprotect(string protectedAccessCode);
}
