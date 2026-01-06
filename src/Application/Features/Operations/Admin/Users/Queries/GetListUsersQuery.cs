using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Admin.Users.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Users.Queries;

public sealed record GetListUsersQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<UserListItemDto>>>
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public bool? IsBlocked { get; init; }
}
