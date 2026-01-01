using FluentResults;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Domain.Constants;
using System.Linq;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Handlers;

internal static class RolePermissionHelper
{
    private static readonly HashSet<string> AllowedPermissions =
        PermissionNames.All.ToHashSet(StringComparer.OrdinalIgnoreCase);

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
