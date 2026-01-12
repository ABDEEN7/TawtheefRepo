using Application.Operation.Features.Admin.Roles.DTOs;
using Application.Operation.Features.Admin.Roles.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Security;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Roles.Handlers.Queries;

public sealed class ListRolesQueryHandler(
    RoleManager<ApplicationRole> roleManager,
    IMapper mapper,
    IUnitOfWork uow,
    ILocalizationService localizationService)
    : IQueryHandler<GetListRolesQuery, IResult<PaginatedResult<RoleDto>>>
{
    public async Task<IResult<PaginatedResult<RoleDto>>> Handle(GetListRolesQuery request, CancellationToken cancellationToken)
    {
        var rolesPage = await roleManager.Roles
            .AsNoTracking()
            .ToPaginatedListAsync(request, cancellationToken);

        var permissionLookup = await uow.GetEntityRepository<Permission>().DbSet
            .AsNoTracking()
            .Where(x => x.IsActive)
            .ToDictionaryAsync(p => p.BackendName, p => localizationService.GetLocalizedName(p), cancellationToken);

        var mapped = new List<RoleDto>(rolesPage.Items.Count);
        foreach (var role in rolesPage.Items)
        {
            var claims = await roleManager.GetClaimsAsync(role);
            var roleDto = mapper.Map<RoleDto>(new RoleWithClaims(role, claims));

            if (roleDto.Permissions.Count > 0)
            {
                roleDto = roleDto with
                {
                    PermissionNames = roleDto.Permissions
                        .Select(p => permissionLookup.TryGetValue(p, out var name) ? name : p)
                        .ToArray()
                };
            }

            mapped.Add(roleDto);
        }

        var metadata = rolesPage.Metadata;
        return Result.Ok(new PaginatedResult<RoleDto>(mapped, metadata.TotalCount, metadata.CurrentPage, metadata.PageSize));
    }
}
