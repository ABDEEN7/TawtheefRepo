using System.Security.Claims;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Roles.Queries;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Handlers.Queries;

public sealed class ListRoleLookupsQueryHandler(RoleManager<IdentityRole<Guid>> roleManager, IMapper mapper)
    : IRequestHandler<ListRoleLookupsQuery, IResult<IReadOnlyCollection<RoleLookupDto>>>
{
    public async Task<IResult<IReadOnlyCollection<RoleLookupDto>>> Handle(
        ListRoleLookupsQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.AsNoTracking().ToListAsync(cancellationToken);

        return Result.Ok(mapper.Map<List<RoleLookupDto>>(roles));
    }

    private static string GetNameClaim(IEnumerable<Claim> claims, string type, string? fallback)
    {
        return claims.FirstOrDefault(c => c.Type == type)?.Value ?? fallback ?? string.Empty;
    }
}
