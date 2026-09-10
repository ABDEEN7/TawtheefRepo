using FluentResults;
using MediatR;

namespace Application.Operation.Features.Interview.EvaluationBank.Commands;

public sealed record ChangeAxisActivationCommand(Guid Id, bool IsActive) : IRequest<IResult<Unit>>;
