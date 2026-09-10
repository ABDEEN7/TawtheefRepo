using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Commands;

public sealed record SubmitTemplateVersionCommand(Guid Id) : IRequest<IResult<Unit>>;
