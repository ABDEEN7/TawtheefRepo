using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Commands;

public sealed record ChangeTemplateActivationCommand(Guid Id, bool IsActive) : IRequest<IResult<Unit>>;
