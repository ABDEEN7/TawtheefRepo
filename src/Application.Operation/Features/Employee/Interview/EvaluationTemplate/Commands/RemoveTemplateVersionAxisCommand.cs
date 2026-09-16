using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Commands;

public sealed record RemoveTemplateVersionAxisCommand(Guid Id) : IRequest<IResult<Unit>>;
