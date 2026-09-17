using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.OperationalIssue.Commands;

public sealed record WaiveOperationalIssueCommand(Guid IssueId, string? ResolutionNotes) : IRequest<IResult<Unit>>;
