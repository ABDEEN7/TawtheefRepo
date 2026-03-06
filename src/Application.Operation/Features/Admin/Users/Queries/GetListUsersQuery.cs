using Application.Operation.Features.Admin.Users.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Admin.Users.Queries;

public sealed record GetListUsersQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<UserListItemDto>>>
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public bool? IsBlocked { get; init; }
}

