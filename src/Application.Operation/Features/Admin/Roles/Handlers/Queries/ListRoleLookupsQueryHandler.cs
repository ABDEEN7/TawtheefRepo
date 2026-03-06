using Application.Operation.Features.Admin.Roles.DTOs;
using Application.Operation.Features.Admin.Roles.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Roles.Handlers.Queries;

public sealed class ListRoleLookupsQueryHandler(RoleManager<ApplicationRole> roleManager, IMapper mapper)
    : IRequestHandler<ListRoleLookupsQuery, IResult<IReadOnlyCollection<RoleLookupDto>>>
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

