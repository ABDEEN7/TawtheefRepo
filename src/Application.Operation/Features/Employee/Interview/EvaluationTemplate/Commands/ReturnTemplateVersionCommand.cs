using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Commands;

public sealed record ReturnTemplateVersionCommand(Guid Id, string Reason) : IRequest<IResult<Unit>>;
