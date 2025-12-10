using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Admin.Users.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Users.Queries;
using Tawtheef.Domain.Constants;
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
        var usersQuery = userManager.Users.AsNoTracking()
            .Where(u => u.UserTypeId == UserTypeIds.Employee && !u.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var nameTerm = $"%{request.Name.Trim()}%";
            usersQuery = usersQuery.Where(u =>
                EF.Functions.Like(u.FullNameEn, nameTerm) ||
                EF.Functions.Like(u.FullNameAr, nameTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailTerm = $"%{request.Email.Trim()}%";
            usersQuery = usersQuery.Where(u => u.Email != null && EF.Functions.Like(u.Email, emailTerm));
        }

        if (request.IsBlocked.HasValue)
            usersQuery = usersQuery.Where(u => u.IsBlocked == request.IsBlocked.Value);

        var totalCount = await usersQuery.CountAsync(cancellationToken);

        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        var skip = (pageNumber - 1) * pageSize;

        var users = await usersQuery
            .OrderBy(u => u.FullNameEn)
            .ThenBy(u => u.Email)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = new List<UserListItemDto>(users.Count);
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            var dto = mapper.Map<UserListItemDto>(user) with
            {
                Roles = roles.ToArray()
            };
            items.Add(dto);
        }

        return Result.Ok(new PaginatedResult<UserListItemDto>(items, totalCount, pageNumber, pageSize));
    }
}
