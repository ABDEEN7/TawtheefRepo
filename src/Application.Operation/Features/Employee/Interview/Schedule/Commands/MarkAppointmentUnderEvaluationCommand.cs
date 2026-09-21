using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Commands;

public sealed record MarkAppointmentUnderEvaluationCommand(Guid AppointmentId) : IRequest<IResult<Unit>>;
