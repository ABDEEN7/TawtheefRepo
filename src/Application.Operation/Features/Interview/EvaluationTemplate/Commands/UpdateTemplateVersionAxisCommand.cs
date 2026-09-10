using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Commands;

public sealed record UpdateTemplateVersionAxisCommand(
    Guid Id,
    decimal MaxScore,
    decimal? QualificationScore,
    int OrderNo) : IRequest<IResult<Unit>>;
