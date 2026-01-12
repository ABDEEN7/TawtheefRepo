using Application.Operation.Features.Admin.Users.DTOs;
using Application.Operation.Features.Admin.Users.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Users.Handlers.Queries;

public sealed class ListUsersQueryHandler(
    UserManager<User> userManager,
    RoleManager<ApplicationRole> roleManager)
    : IQueryHandler<GetListUsersQuery, IResult<PaginatedResult<UserListItemDto>>>
{
    public async Task<IResult<PaginatedResult<UserListItemDto>>> Handle(
        GetListUsersQuery request,
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

        var totalCount = await queryable.CountAsync(cancellationToken);

        var users = await queryable
            .OrderBy(u => u.FullNameEn)
            .ThenBy(u => u.Email)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var roleEntities = await roleManager.Roles
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var items = new List<UserListItemDto>(users.Count);
        foreach (var user in users)
        {
            var assignedRoleNames = await userManager.GetRolesAsync(user);
            var assignedRoles = roleEntities
                .Where(r => assignedRoleNames.Contains(r.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase))
                .Select(r => new RoleSummaryDto
                {
                    Id = r.Id,
                    NameAr = r.NameAr ?? r.Name ?? string.Empty,
                    NameEn = r.NameEn ?? r.Name ?? string.Empty,
                    SystemName = r.Name ?? string.Empty,
                    IsSystemRole = r.IsSystemRole
                })
                .ToArray();

            items.Add(new UserListItemDto
            {
                Id = user.Id,
                Name = string.IsNullOrWhiteSpace(user.FullNameAr) ? user.FullNameEn : user.FullNameAr,
                Email = user.Email ?? string.Empty,
                LastLoginDate = user.LastLoginDate,
                IsBlocked = user.IsBlocked,
                Roles = assignedRoles,
                RoleNames = assignedRoleNames.ToList()
            });
        }

        var result = new PaginatedResult<UserListItemDto>(items, totalCount, request.PageNumber, request.PageSize);

        return Result.Ok(result);
    }
}
