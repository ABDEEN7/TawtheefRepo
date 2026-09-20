using Microsoft.AspNetCore.DataProtection;
using Tawtheef.Application.Common.Interfaces.Services.Security;

namespace Tawtheef.Infrastructure.Services.Security;

public sealed class AccessCodeProtector(IDataProtectionProvider dataProtectionProvider) : IAccessCodeProtector
{
    private readonly IDataProtector protector = dataProtectionProvider.CreateProtector("Tawtheef.TestSlots.AccessCode.v1");

    public string Protect(string accessCode) => protector.Protect(accessCode);

    public string Unprotect(string protectedAccessCode) => protector.Unprotect(protectedAccessCode);
}
