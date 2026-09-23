using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Queries;

// Session-view equivalent of Schedule.GetScheduleByIdQuery, but access-checked against the caller's
// own committee membership (or bypass) instead of the InterviewSchedule permission
public sealed record GetMySessionQuery(Guid ScheduleId) : IRequest<IResult<ScheduleDto>>;
