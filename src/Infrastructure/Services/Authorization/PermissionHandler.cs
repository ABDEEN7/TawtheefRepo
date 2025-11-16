using Microsoft.AspNetCore.Authorization;
using Tawtheef.Application.Common.Constants;

namespace Tawtheef.Infrastructure.Services.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.HasClaim("permission", requirement.Permission))
        {
            context.Succeed(requirement);
        }

        if (context.User.IsInRole(RoleNames.Admin))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
