using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Commands;

public sealed record AddTemplateVersionAxisCommand(
    Guid InterviewTemplateVersionId,
    Guid InterviewEvaluationAxisId,
    decimal MaxScore,
    decimal? QualificationScore,
    int OrderNo) : IRequest<IResult<Guid>>;
