using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Commands;

public sealed record StartAppointmentInterviewCommand(Guid AppointmentId) : IRequest<IResult<Unit>>;
