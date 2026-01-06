using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Roles.Queries;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Handlers.Queries;

public sealed class ListRoleLookupsQueryHandler(RoleManager<ApplicationRole> roleManager, IMapper mapper)
    : IQueryHandler<ListRoleLookupsQuery, IResult<IReadOnlyCollection<RoleLookupDto>>>
{
    public async Task<IResult<IReadOnlyCollection<RoleLookupDto>>> Handle(
        ListRoleLookupsQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Ok(mapper.Map<List<RoleLookupDto>>(roles));
    }
}
