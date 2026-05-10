using System.Security.Claims;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Security;

public static class ClaimsPrincipalExtensions
{
    public static bool HasFullJobAccess(this ClaimsPrincipal user)
    {
        if (user == null) return false;

        var isHrManager = user.IsInRole(nameof(SystemRoleIds.HrManager));
        if (isHrManager) return true;

        var permissions = user.Claims
            .Where(c => c.Type == RoleClaimTypes.Permission)
            .Select(c => c.Value)
            .ToList();

        var crudPermissions = new[]
        {
            PermissionKeys.Jobs.View,
            PermissionKeys.Jobs.Edit,
            PermissionKeys.Jobs.Create,
            PermissionKeys.Jobs.Delete
        };

        var hasOtherJobPermissions = permissions.Any(p => 
            (p.StartsWith("jobs.") || p.StartsWith("jobs.points.")) && 
            !crudPermissions.Contains(p));

        return hasOtherJobPermissions;
    }
}
