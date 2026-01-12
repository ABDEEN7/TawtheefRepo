using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Roles.Handlers;

internal static class RoleClaimSync
{
    public static Result<T> FailureFromIdentity<T>(IdentityResult res)
        => Result.Fail<T>(string.Join(", ", res.Errors.Select(e => e.Description)));

    public static Result FailureFromIdentity(IdentityResult res)
        => Result.Fail(string.Join(", ", res.Errors.Select(e => e.Description)));

    public static async Task<Result> SetNameClaimsAsync(
        RoleManager<ApplicationRole> roleManager,
        ApplicationRole role,
        string nameAr,
        string nameEn,
        IEnumerable<Claim>? existingClaims = null)
    {
        var claims = existingClaims?.ToList() ?? (await roleManager.GetClaimsAsync(role)).ToList();

        foreach (var claim in claims.Where(c => c.Type == RoleClaimTypes.NameArabic || c.Type == RoleClaimTypes.NameEnglish))
        {
            var remove = await roleManager.RemoveClaimAsync(role, claim);
            if (!remove.Succeeded) return FailureFromIdentity(remove);
        }

        var arResult = await roleManager.AddClaimAsync(role, new Claim(RoleClaimTypes.NameArabic, nameAr));
        if (!arResult.Succeeded) return FailureFromIdentity(arResult);

        var enResult = await roleManager.AddClaimAsync(role, new Claim(RoleClaimTypes.NameEnglish, nameEn));
        if (!enResult.Succeeded) return FailureFromIdentity(enResult);

        return Result.Ok();
    }

    public static async Task<Result> SyncPermissionsAsync(
        RoleManager<ApplicationRole> roleManager,
        ApplicationRole role,
        IEnumerable<string> existingPermissions,
        IEnumerable<string> desiredPermissions)
    {
        var existingSet = existingPermissions.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var desiredSet = desiredPermissions.ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var permission in existingSet.Except(desiredSet))
        {
            var remove = await roleManager.RemoveClaimAsync(role, new Claim(RoleClaimTypes.Permission, permission));
            if (!remove.Succeeded) return FailureFromIdentity(remove);
        }

        foreach (var permission in desiredSet.Except(existingSet))
        {
            var add = await roleManager.AddClaimAsync(role, new Claim(RoleClaimTypes.Permission, permission));
            if (!add.Succeeded) return FailureFromIdentity(add);
        }

        return Result.Ok();
    }
}
