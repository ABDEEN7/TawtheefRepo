using FluentResults;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Handlers;

internal static class RolePermissionHelper
{
    public static Result<List<string>> Validate(IEnumerable<string> permissions)
    {
        var cleaned = permissions?
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? [];

        var invalid = cleaned.Where(p => !PermissionCatalog.Keys.Contains(p)).ToList();

        return invalid.Count > 0
            ? Result.Fail<List<string>>($"{ErrorsCodes.InvalidPermission}: {string.Join(", ", invalid)}")
            : Result.Ok(cleaned);
    }
}
