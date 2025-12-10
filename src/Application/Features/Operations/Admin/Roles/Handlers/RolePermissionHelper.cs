using FluentResults;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Handlers;

internal static class RolePermissionHelper
{
    private static readonly HashSet<string> AllowedPermissions = new([
        PermissionNames.UsersView,
        PermissionNames.UsersManage,
        PermissionNames.ProfileView,
        PermissionNames.ProfileManage,
        PermissionNames.JobsView,
        PermissionNames.JobsManage
    ], StringComparer.OrdinalIgnoreCase);

    public static Result<List<string>> Validate(IEnumerable<string> permissions)
    {
        var cleaned = permissions?
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? [];

        var invalid = cleaned.Where(p => !AllowedPermissions.Contains(p)).ToList();
        return invalid.Count > 0
            ? Result.Fail<List<string>>($"{ErrorsCodes.InvalidPermission}: {string.Join(", ", invalid)}")
            : Result.Ok(cleaned);
    }
}
