using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Commands;

public sealed record SubmitMemberEvaluationCommand(Guid AppointmentId) : IRequest<IResult<Unit>>;
