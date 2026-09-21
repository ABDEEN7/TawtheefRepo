using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Commands;

public sealed record CloseAppointmentCommand(Guid AppointmentId) : IRequest<IResult<Unit>>;
