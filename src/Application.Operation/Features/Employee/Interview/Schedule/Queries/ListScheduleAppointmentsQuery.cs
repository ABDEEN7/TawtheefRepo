using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Queries;

public sealed record ListScheduleAppointmentsQuery(Guid InterviewScheduleId) : IRequest<IResult<List<AppointmentDto>>>;
