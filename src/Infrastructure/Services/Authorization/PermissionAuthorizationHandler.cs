using Microsoft.AspNetCore.Authorization;
using Tawtheef.Application.Common.Security;

namespace Tawtheef.Infrastructure.Services.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (string.IsNullOrWhiteSpace(requirement.Permission))
        {
            return Task.CompletedTask;
        }

        var permissions = requirement.Permission.Split(
            '|',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (permissions.Any(permission => context.User.HasClaim("permission", permission)))
            context.Succeed(requirement);
        
        return Task.CompletedTask;
    }
}
