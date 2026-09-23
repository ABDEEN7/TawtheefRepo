using Application.Operation.Features.Employee.Interview.Evaluation.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Queries;

// Lets the frontend decide what to show (Attendance action, committee summary) before attempting
// the CanSubmitEvaluation-gated form or the Chair/summary-gated ListAppointmentEvaluations - both
// of those 403 for callers this query itself resolves cleanly.
public sealed record GetAppointmentEvaluationContextQuery(Guid AppointmentId) : IRequest<IResult<AppointmentEvaluationContextDto>>;
