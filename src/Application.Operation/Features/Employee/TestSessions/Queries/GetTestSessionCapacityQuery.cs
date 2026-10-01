using Application.Operation.Features.Employee.TestSessions.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSessions.Queries;

public sealed record GetTestSessionCapacityQuery(Guid TestSlotId, TimeOnly StartTime, TimeOnly EndTime,
    int SelectedCandidateCount) : IRequest<IResult<TestSessionCapacityDto>>;
