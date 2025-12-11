using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Roles.Queries;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Handlers.Queries;

public sealed class ListRoleLookupsQueryHandler(RoleManager<IdentityRole<Guid>> roleManager)
    : IRequestHandler<ListRoleLookupsQuery, IResult<IReadOnlyCollection<RoleLookupDto>>>
{
    public async Task<IResult<IReadOnlyCollection<RoleLookupDto>>> Handle(
        ListRoleLookupsQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.AsNoTracking().ToListAsync(cancellationToken);

        var lookupDtos = new List<RoleLookupDto>(roles.Count);

        foreach (var role in roles)
        {
            var claims = await roleManager.GetClaimsAsync(role);
            lookupDtos.Add(new RoleLookupDto
            {
                Id = role.Id,
                NameAr = GetNameClaim(claims, RoleClaimTypes.NameArabic, role.Name),
                NameEn = GetNameClaim(claims, RoleClaimTypes.NameEnglish, role.Name)
            });
        }

        return Result.Ok<IReadOnlyCollection<RoleLookupDto>>(lookupDtos);
    }

    private static string GetNameClaim(IEnumerable<Claim> claims, string type, string? fallback)
    {
        return claims.FirstOrDefault(c => c.Type == type)?.Value ?? fallback ?? string.Empty;
    }
}
