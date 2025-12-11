using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Admin.Users.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Users.Queries;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Users.Handlers.Queries;

public sealed class ListUsersQueryHandler(UserManager<User> userManager, IMapper mapper)
    : IRequestHandler<ListUsersQuery, IResult<PaginatedResult<UserListItemDto>>>
{
    public async Task<IResult<PaginatedResult<UserListItemDto>>> Handle(
        ListUsersQuery request,
        CancellationToken cancellationToken)
    {
        var queryable = userManager.Users
            .AsNoTracking()
            .Where(u => u.UserTypeId == UserTypeIds.Employee && !u.IsDeleted)
            .WhereIf(request.Name is not null,
                i => EF.Functions.Like(i.FullNameEn, $"%{request.Name!.Trim()}%") ||
                     EF.Functions.Like(i.FullNameAr, $"%{request.Name!.Trim()}%"))
            .WhereIf(request.Email is not null,
                i => i.Email != null &&
                     EF.Functions.Like(i.Email!, $"%{request.Email!.Trim()}%"))
            .WhereIf(request.IsBlocked.HasValue,
                i => i.IsBlocked == request.IsBlocked!.Value);
        
        var result = await queryable
            .OrderBy(u => u.FullNameEn)
            .ThenBy(u => u.Email)
            .ToPaginatedListAsync<User, UserListItemDto>(mapper, request, cancellationToken);


        var userIds = result.Items.Select(u => u.Id).ToList();

        var rolesLookup = await userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new
            {
                u.Id,
                Roles = userManager.GetRolesAsync(u)
            })
            .ToListAsync(cancellationToken);

        var roleDict = new Dictionary<Guid, string[]>();

        foreach (var item in rolesLookup)
        {
            roleDict[item.Id] = (await item.Roles).ToArray();
        }

        foreach (var dto in result.Items)
        {
            if (roleDict.TryGetValue(dto.Id, out var roles))
                dto.Roles = roles;
        }

        return Result.Ok(result);
    }
}
