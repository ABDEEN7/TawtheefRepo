using Application.Operation.Features.Employee.CandidateUsers.Contracts;
using Application.Operation.Features.Employee.CandidateUsers.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.CandidateUsers.Queries;

public sealed record GetCandidateUsersQuery
    : PaginatedRequest,
        ICandidateUsersFilter,
        IRequest<IResult<PaginatedResult<CandidateUserListItemDto>>>
{
    public string? Search { get; init; }

    public bool? IsBlocked { get; init; }

    public List<UserProfileStatus>? ProfileStatuses { get; init; }

    // Existing dashboard/deep-link filter.
    public UserProfileStatus? ProfileStatus { get; init; }

    public int? Year { get; init; }

    public CandidateUsersResultScope Scope { get; init; } =
        CandidateUsersResultScope.Default;
}
