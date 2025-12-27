using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Constants;

public static class RolePermissionSeeder
{
    private const string PermClaimType = "perm";

    public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager)
    {
        // Example role names (adjust to yours)
        await EnsureRoleHasPerms(roleManager, "Admin", GetAllPermissions());
        await EnsureRoleHasPerms(roleManager, "HR", [
            PermissionNames.UsersView,
            PermissionNames.ProfileView,
            PermissionNames.JobsView,
            PermissionNames.JobsManage
        ]);

        await EnsureRoleHasPerms(roleManager, "Viewer", [
            PermissionNames.UsersView,
            PermissionNames.ProfileView,
            PermissionNames.JobsView
        ]);
    }

    private static async Task EnsureRoleHasPerms(
        RoleManager<ApplicationRole> roleManager,
        string roleName,
        IEnumerable<string> perms)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null) return;

        var existingClaims = await roleManager.GetClaimsAsync(role);
        var existingPerms = existingClaims
            .Where(c => c.Type == PermClaimType)
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var perm in perms.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!existingPerms.Contains(perm))
                await roleManager.AddClaimAsync(role, new Claim(PermClaimType, perm));
        }
    }

    private static IEnumerable<string> GetAllPermissions()
    {
        return PermissionNames.All;
    }
}
