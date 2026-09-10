using System;
using FluentResults;
using MediatR;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Commands;

public sealed record CreateTemplateVersionCommand(
    Guid InterviewTemplateId,
    decimal FinalScore,
    decimal? QualificationScore,
    CalculationMethod CalculationMethod) : IRequest<IResult<Guid>>;
