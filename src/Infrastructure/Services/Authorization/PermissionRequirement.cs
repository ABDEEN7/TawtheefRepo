using Microsoft.AspNetCore.Authorization;

namespace Tawtheef.Infrastructure.Services.Authorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
