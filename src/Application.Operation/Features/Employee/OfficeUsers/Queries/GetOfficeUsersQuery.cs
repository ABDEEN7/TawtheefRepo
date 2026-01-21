using Application.Operation.Features.Employee.OfficeUsers.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.OfficeUsers.Queries;

public sealed record GetOfficeUsersQuery
    : PaginatedRequest,
        IQuery<IResult<PaginatedResult<OfficeUserListItemDto>>>
{
    public string? Name { get; init; }
}
