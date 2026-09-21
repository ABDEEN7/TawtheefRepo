using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Commands;

public sealed record ApproveScheduleCommand(Guid Id, string? DecisionNotes) : IRequest<IResult<Unit>>;
