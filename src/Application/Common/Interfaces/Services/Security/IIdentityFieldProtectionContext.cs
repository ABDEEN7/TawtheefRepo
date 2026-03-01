namespace Tawtheef.Application.Common.Interfaces.Services.Security;

public interface IIdentityFieldProtectionContext
{
    bool AllowVerifiedIdentityWrite { get; }
    IDisposable BeginTrustedIdentityWriteScope();
}
