using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Schedule.Queries;

// Feeds wizard steps 1+2: job identity fields, the job's already-created (Approved) committee and
// its roster, and eligible-candidate pool stats for the step-2 cards.
public sealed record GetScheduleCreationContextQuery(Guid JobId) : IRequest<IResult<ScheduleCreationContextDto>>;
