using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.OperationalIssue.Commands;

public sealed record UpdateOperationalIssueBlockingCommand(Guid IssueId, bool IsBlocking) : IRequest<IResult<Unit>>;
