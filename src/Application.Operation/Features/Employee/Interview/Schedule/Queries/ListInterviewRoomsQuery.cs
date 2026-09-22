using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Queries;

// Feeds the Step 2 In-Person period dialog's room picker. Deliberately not paginated: the caller
// narrows the list by typing and the handler caps the result, matching
// ListEligibleCommitteeMembersQuery's convention.
public sealed record ListInterviewRoomsQuery(string? Search) : IRequest<IResult<List<RoomOptionDto>>>;
