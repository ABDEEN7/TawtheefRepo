using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Handlers;

internal static class RoleMapping
{
    public static RoleDto ToDto(IdentityRole<Guid> role, IEnumerable<Claim> claims)
    {
        var claimList = claims.ToList();
        var nameAr = claimList.FirstOrDefault(c => c.Type == RoleClaimTypes.NameArabic)?.Value ?? role.Name ?? string.Empty;
        var nameEn = claimList.FirstOrDefault(c => c.Type == RoleClaimTypes.NameEnglish)?.Value ?? role.Name ?? string.Empty;

        var permissions = claimList
            .Where(c => c.Type == RoleClaimTypes.Permission)
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new RoleDto
        {
            Id = role.Id,
            NameAr = nameAr,
            NameEn = nameEn,
            Permissions = permissions
        };
    }
}
