using Microsoft.Extensions.DependencyInjection;

namespace Tawtheef.Application.Common.Security;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddPermissionPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            foreach (var perm in PermissionCatalog.All)
            {
                var policyName = PermissionPolicyProvider.PolicyPrefix + perm.Key.Value;

                options.AddPolicy(policyName, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(RoleClaimTypes.Permission, perm.Key.Value);
                });
            }
        });

        return services;
    }
}
