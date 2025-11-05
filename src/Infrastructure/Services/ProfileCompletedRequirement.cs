using Microsoft.AspNetCore.Authorization;

namespace Tawtheef.Infrastructure.Services;

public sealed class ProfileCompletedRequirement : IAuthorizationRequirement {}

public sealed class ProfileCompletedHandler : AuthorizationHandler<ProfileCompletedRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProfileCompletedRequirement requirement)
    {
        var claim = context.User.FindFirst("profile.completed");
        if (claim?.Value == "true")
            context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
