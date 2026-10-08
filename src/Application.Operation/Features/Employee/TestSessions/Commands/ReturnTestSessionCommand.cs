using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSessions.Commands;

public sealed record ReturnTestSessionCommand(Guid TestSessionId, string? DecisionNote)
    : IRequest<IResult<Unit>>;
