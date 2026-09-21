using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Queries;

public sealed record GetScheduleCreationContextQuery(Guid JobId, Guid? ExcludeScheduleId = null)
    : IRequest<IResult<ScheduleCreationContextDto>>;
