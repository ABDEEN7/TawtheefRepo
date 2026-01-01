using Microsoft.AspNetCore.Authorization;

namespace Tawtheef.Application.Common.Security;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
