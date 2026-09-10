using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Commands;

public sealed record ReturnTemplateVersionCommand(Guid Id, string Reason) : IRequest<IResult<Unit>>;
