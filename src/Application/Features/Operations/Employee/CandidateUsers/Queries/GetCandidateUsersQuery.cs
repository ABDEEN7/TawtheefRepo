using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.CandidateUsers.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.CandidateUsers.Queries;

public sealed record GetCandidateUsersQuery
    : PaginatedRequest,
        IQuery<IResult<PaginatedResult<CandidateUserListItemDto>>>
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Qid { get; init; }
    public string? MobileNumber { get; init; }
}
