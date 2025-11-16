using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Tawtheef.Infrastructure.Services.Authorization;

public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    public const string POLICY_PREFIX = "Permission:";

    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
        {
            var permission = policyName.Substring(POLICY_PREFIX.Length);

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(permission))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        // باقي الـ Policies العادية
        return base.GetPolicyAsync(policyName);
    }
}
