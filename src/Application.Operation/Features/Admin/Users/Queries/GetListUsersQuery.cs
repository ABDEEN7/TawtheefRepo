using Application.Operation.Features.Admin.Users.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Admin.Users.Queries;

public sealed record GetListUsersQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<UserListItemDto>>>
{
    public string? Search { get; init; }
    public bool? IsBlocked { get; init; }
}

