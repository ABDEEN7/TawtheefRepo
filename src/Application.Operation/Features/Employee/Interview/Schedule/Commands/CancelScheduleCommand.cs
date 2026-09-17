using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Commands;

public sealed record CancelScheduleCommand(Guid Id, string Reason) : IRequest<IResult<Unit>>;
