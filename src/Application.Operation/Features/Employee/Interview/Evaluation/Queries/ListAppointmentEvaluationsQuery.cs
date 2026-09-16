using Application.Operation.Features.Employee.Interview.Evaluation.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Queries;

// Chair/HR summary of every committee member's evaluation status+total for one appointment -
// distinct access rule from the "my own form" queries/commands, see ListAppointmentEvaluationsQueryHandler.
public sealed record ListAppointmentEvaluationsQuery(Guid AppointmentId) : IRequest<IResult<AppointmentEvaluationSummaryDto>>;
