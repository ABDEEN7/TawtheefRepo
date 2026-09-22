using Application.Operation.Features.Employee.TestSlots.DTOs;
using Application.Operation.Features.Employee.TestSlots.Queries;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Queries;

public sealed class GetTestSlotStaffMembersQueryHandler(
    UserManager<User> userManager,
    RoleManager<ApplicationRole> roleManager,
    ILocalizationService localizationService)
    : IRequestHandler<GetTestSlotStaffMembersQuery, IResult<PaginatedResult<TestSlotStaffMemberDto>>>
{
    public async Task<IResult<PaginatedResult<TestSlotStaffMemberDto>>> Handle(
        GetTestSlotStaffMembersQuery request,
        CancellationToken cancellationToken)
    {
        var roleId = await roleManager.Roles
            .Where(role => role.Name == nameof(SystemRoleIds.TestSlotStaffMember))
            .Select(role => (Guid?)role.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (!roleId.HasValue)
            return Result.Ok(new PaginatedResult<TestSlotStaffMemberDto>([], 0, request.PageNumber, request.PageSize));

        var testSlotStaffRoleId = roleId.Value;
        var queryable = userManager.Users
            .AsNoTracking()
            .Where(user => user.UserTypeId == UserTypeIds.Employee && !user.IsDeleted)
            .Where(user => user.UserRoles.Any(userRole => userRole.RoleId == testSlotStaffRoleId))
            .WhereIf(!string.IsNullOrWhiteSpace(request.Search),
                user => EF.Functions.Like(user.FullNameAr, $"%{request.Search!.Trim()}%") ||
                        EF.Functions.Like(user.FullNameEn, $"%{request.Search!.Trim()}%") ||
                        (user.Email != null && EF.Functions.Like(user.Email, $"%{request.Search!.Trim()}%")) ||
                        (user is EmployeeUser &&
                         (user as EmployeeUser)!.EmployeeProfile != null &&
                         (user as EmployeeUser)!.EmployeeProfile!.Department != null &&
                         EF.Functions.Like(
                             (user as EmployeeUser)!.EmployeeProfile!.Department!,
                             $"%{request.Search!.Trim()}%")));

        if (request.IsBlocked is { } isBlocked)
            queryable = queryable.Where(user => user.IsBlocked == isBlocked);

        var totalCount = await queryable.CountAsync(cancellationToken);
        var users = await queryable
            .OrderBy(user => user.FullNameEn)
            .ThenBy(user => user.Email)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(user => new
            {
                User = user,
                Department = user is EmployeeUser && (user as EmployeeUser)!.EmployeeProfile != null
                    ? (user as EmployeeUser)!.EmployeeProfile!.Department
                    : null,
                JobTitle = user is EmployeeUser && (user as EmployeeUser)!.EmployeeProfile != null
                    ? (user as EmployeeUser)!.EmployeeProfile!.JobTitle
                    : null,
            })
            .ToListAsync(cancellationToken);

        var items = users.Select(row => new TestSlotStaffMemberDto
        {
            Id = row.User.Id,
            Name = localizationService.GetLocalizedFullName(row.User),
            Email = row.User.Email ?? string.Empty,
            JobTitle = string.IsNullOrWhiteSpace(row.JobTitle) ? null : row.JobTitle,
            DepartmentName = string.IsNullOrWhiteSpace(row.Department) ? null : row.Department,
            IsBlocked = row.User.IsBlocked,
        }).ToList();

        return Result.Ok(new PaginatedResult<TestSlotStaffMemberDto>(items, totalCount, request.PageNumber, request.PageSize));
    }
}
