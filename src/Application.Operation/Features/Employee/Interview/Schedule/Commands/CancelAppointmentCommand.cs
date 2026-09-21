using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Commands;

public sealed record CancelAppointmentCommand(Guid AppointmentId, string Reason) : IRequest<IResult<Unit>>;
