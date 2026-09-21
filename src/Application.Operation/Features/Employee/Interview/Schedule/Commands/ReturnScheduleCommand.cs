using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Commands;

public sealed record ReturnScheduleCommand(Guid Id, string Reason) : IRequest<IResult<Unit>>;
