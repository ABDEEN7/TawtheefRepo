using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSessions.Commands;

public sealed record ApproveTestSessionCommand(Guid TestSessionId) : IRequest<IResult<Unit>>;
