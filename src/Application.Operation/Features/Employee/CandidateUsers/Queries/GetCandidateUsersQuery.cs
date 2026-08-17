using Application.Operation.Features.Employee.CandidateUsers.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.CandidateUsers.Queries;

public sealed record GetCandidateUsersQuery
    : PaginatedRequest, IRequest<IResult<PaginatedResult<CandidateUserListItemDto>>>
{
    public string? Search { get; init; }
    public bool? IsBlocked { get; init; }
    public IReadOnlyCollection<UserProfileStatus>? ProfileStatuses { get; init; }

    // Retained for compatibility with existing API consumers.
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Qid { get; init; }
    public string? MobileNumber { get; init; }
}

