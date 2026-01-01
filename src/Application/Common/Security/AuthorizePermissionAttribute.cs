namespace Tawtheef.Application.Common.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class AuthorizePermissionAttribute : Microsoft.AspNetCore.Authorization.AuthorizeAttribute
{
    // public AuthorizePermissionAttribute(PermissionDefinition permission)
    //     => Policy = PermissionPolicyProvider.PolicyPrefix + permission.Key.Value;
    //
    // public AuthorizePermissionAttribute(PermissionKey permission)
    //     => Policy = PermissionPolicyProvider.PolicyPrefix + permission.Value;
    public AuthorizePermissionAttribute(string permissionKey)
    {
        Policy = PermissionPolicyProvider.PolicyPrefix + permissionKey;
    }
}
