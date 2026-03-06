using Application.Operation.Features.Employee.OfficeUsers.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.OfficeUsers.Queries;

public sealed record GetOfficeUsersQuery
    : PaginatedRequest,
        IRequest<IResult<PaginatedResult<OfficeUserListItemDto>>>
{
    public string? Name { get; init; }
}

