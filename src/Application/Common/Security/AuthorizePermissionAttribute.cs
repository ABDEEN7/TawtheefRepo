namespace Tawtheef.Application.Common.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class AuthorizePermissionAttribute : Microsoft.AspNetCore.Authorization.AuthorizeAttribute
{
    // public AuthorizePermissionAttribute(PermissionDefinition permission)
    //     => Policy = PermissionPolicyProvider.PolicyPrefix + permission.Key.Value;
    //
    // public AuthorizePermissionAttribute(PermissionKey permission)
    //     => Policy = PermissionPolicyProvider.PolicyPrefix + permission.Value;
    public AuthorizePermissionAttribute(params string[] permissionKeys)
    {
        if (permissionKeys is null || permissionKeys.Length == 0)
        {
            throw new ArgumentException("At least one permission key must be provided.", nameof(permissionKeys));
        }

        var policyValue = string.Join('|', permissionKeys);
        Policy = PermissionPolicyProvider.PolicyPrefix + policyValue;
    }
}
