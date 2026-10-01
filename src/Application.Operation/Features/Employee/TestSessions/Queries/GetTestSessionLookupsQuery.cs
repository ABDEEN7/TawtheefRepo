using Application.Operation.Features.Employee.TestSessions.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSessions.Queries;

public sealed record GetTestSessionLookupsQuery(string Language, Guid? RoomId)
    : IRequest<IResult<TestSessionLookupsDto>>;
