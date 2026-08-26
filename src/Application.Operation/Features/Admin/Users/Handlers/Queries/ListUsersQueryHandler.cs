using Application.Operation.Features.Admin.Users.DTOs;
using Application.Operation.Features.Admin.Users.Queries;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Users.Handlers.Queries;

public sealed class ListUsersQueryHandler(
    UserManager<User> userManager,
    RoleManager<ApplicationRole> roleManager,
    ILocalizationService localizationService)
    : IRequestHandler<GetListUsersQuery, IResult<PaginatedResult<UserListItemDto>>>
{
    public async Task<IResult<PaginatedResult<UserListItemDto>>> Handle(
        GetListUsersQuery request,
        CancellationToken cancellationToken)
    {
        var queryable = userManager.Users
            .AsNoTracking()
            .Where(u => u.UserTypeId == UserTypeIds.Employee && !u.IsDeleted)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Search),
                i => EF.Functions.Like(i.FullNameAr, $"%{request.Search!.Trim()}%") ||
                     EF.Functions.Like(i.FullNameEn, $"%{request.Search!.Trim()}%") ||
                     (i.Email != null && EF.Functions.Like(i.Email, $"%{request.Search!.Trim()}%")) ||
                     (i is EmployeeUser &&
                      (i as EmployeeUser)!.EmployeeProfile != null &&
                      (i as EmployeeUser)!.EmployeeProfile!.Department != null &&
                      EF.Functions.Like(
                          (i as EmployeeUser)!.EmployeeProfile!.Department!,
                          $"%{request.Search!.Trim()}%")))
            .WhereIf(request.IsBlocked.HasValue,
                i => i.IsBlocked == request.IsBlocked!.Value);

        var totalCount = await queryable.CountAsync(cancellationToken);

        var users = await queryable
            .OrderBy(u => u.FullNameEn)
            .ThenBy(u => u.Email)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new
            {
                User = u,
                Department = u is EmployeeUser && (u as EmployeeUser)!.EmployeeProfile != null
                    ? (u as EmployeeUser)!.EmployeeProfile!.Department
                    : null
            })
            .ToListAsync(cancellationToken);

        var roleEntities = await roleManager.Roles
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var items = new List<UserListItemDto>(users.Count);
        foreach (var row in users)
        {
            var user = row.User;
            var assignedRoleNames = await userManager.GetRolesAsync(user);
            var assignedRoles = roleEntities
                .Where(r => assignedRoleNames.Contains(r.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase))
                .Select(r => new RoleSummaryDto
                {
                    Id = r.Id,
                    Name = localizationService.GetLocalizedValue(
                        r.NameAr ?? r.Name ?? string.Empty,
                        r.NameEn ?? r.Name ?? string.Empty),
                    NameAr = r.NameAr ?? r.Name ?? string.Empty,
                    NameEn = r.NameEn ?? r.Name ?? string.Empty,
                    SystemName = r.Name ?? string.Empty,
                    IsSystemRole = r.IsSystemRole
                })
                .ToArray();

            items.Add(new UserListItemDto
            {
                Id = user.Id,
                Name = localizationService.GetLocalizedFullName(user),
                Email = user.Email ?? string.Empty,
                Section = string.IsNullOrWhiteSpace(row.Department) ? null : row.Department,
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

