using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Queries;

public sealed record ListSchedulesQuery(Guid? JobId, ScheduleStatus? Status) : IRequest<IResult<List<ScheduleListItemDto>>>;
