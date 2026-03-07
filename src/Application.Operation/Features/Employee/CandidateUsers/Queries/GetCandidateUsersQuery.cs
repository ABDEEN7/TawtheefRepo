using Application.Operation.Features.Employee.CandidateUsers.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.CandidateUsers.Queries;

public sealed record GetCandidateUsersQuery
    : PaginatedRequest,
        IRequest<IResult<PaginatedResult<CandidateUserListItemDto>>>
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Qid { get; init; }
    public string? MobileNumber { get; init; }
}

