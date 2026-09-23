using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Queries;

// Per-schedule appointments for the sessions list's Progress/Location enrichment (see
// interview-evaluation.facade.ts enrichAndSetSessions) - same access check as GetMySessionQuery.
public sealed record ListMySessionAppointmentsQuery(Guid ScheduleId) : IRequest<IResult<List<AppointmentDto>>>;
