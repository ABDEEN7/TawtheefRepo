using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Roles.Queries;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Handlers.Queries;

public sealed class ListRolesQueryHandler(RoleManager<ApplicationRole> roleManager, IMapper mapper)
    : IRequestHandler<ListRolesQuery, IResult<PaginatedResult<RoleDto>>>
{
    public async Task<IResult<PaginatedResult<RoleDto>>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.AsNoTracking().ToListAsync(cancellationToken);

        var mapped = new List<RoleDto>(roles.Count);
        foreach (var role in roles)
        {
            var claims = await roleManager.GetClaimsAsync(role);
            mapped.Add(mapper.Map<RoleDto>(new RoleWithClaims(role, claims)));
        }

        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? mapped.Count : request.PageSize;
        var skip = (pageNumber - 1) * pageSize;
        var items = mapped.Skip(skip).Take(pageSize).ToList();

        return Result.Ok(new PaginatedResult<RoleDto>(items, mapped.Count, pageNumber, pageSize));
    }
}
