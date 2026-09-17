using FluentResults;
using MediatR;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.OperationalIssue.Commands;

public sealed record CreateOperationalIssueCommand(
    Guid AppointmentId,
    OperationalIssueType IssueType,
    string? Description,
    bool IsBlocking) : IRequest<IResult<Guid>>;
