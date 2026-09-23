using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Queries;

// The Start Interview sessions list, scoped to the caller: every schedule for a Chair/HR/SuperAdmin
// bypass, or only the schedules of jobs whose committee the caller actively sits on otherwise. A
// same-shaped alternative to Schedule.ListSchedulesQuery (which is InterviewSchedule-permission gated
// and unscoped) for the Evaluation stage's own, differently-permissioned page.
public sealed record ListMySessionsQuery : IRequest<IResult<List<ScheduleListItemDto>>>;
