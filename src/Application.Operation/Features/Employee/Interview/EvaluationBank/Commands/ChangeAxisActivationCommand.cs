using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.EvaluationBank.Commands;

public sealed record ChangeAxisActivationCommand(Guid Id, bool IsActive) : IRequest<IResult<Unit>>;
