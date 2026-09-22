using Application.Operation.Features.Employee.Interview.Committee.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Committee.Queries;

    // Feeds the searchable Chair/Member dropdowns.
public sealed record ListEligibleCommitteeMembersQuery(string? Search) : IRequest<IResult<List<EligibleCommitteeMemberDto>>>;
