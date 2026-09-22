using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Commands;

public sealed record SubmitScheduleCommand(Guid Id) : IRequest<IResult<Unit>>;
