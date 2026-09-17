using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Commands;

public sealed record CloseScheduleCommand(Guid Id) : IRequest<IResult<Unit>>;
