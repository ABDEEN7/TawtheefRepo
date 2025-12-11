using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Admin.Users.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Users.Queries;
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

        return Result.Ok(result);
    }
}
