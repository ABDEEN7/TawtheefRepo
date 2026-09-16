using FluentResults;
using MediatR;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Commands;

public sealed record RecordAppointmentAttendanceCommand(Guid AppointmentId, AttendanceStatus AttendanceStatus) : IRequest<IResult<Unit>>;
