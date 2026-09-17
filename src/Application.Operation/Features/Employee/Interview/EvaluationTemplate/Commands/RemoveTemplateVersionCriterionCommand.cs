using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Commands;

public sealed record RemoveTemplateVersionCriterionCommand(Guid Id) : IRequest<IResult<Unit>>;
