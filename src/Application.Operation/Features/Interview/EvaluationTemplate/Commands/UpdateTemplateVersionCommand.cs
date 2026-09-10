using System;
using FluentResults;
using MediatR;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Commands;

public sealed record UpdateTemplateVersionCommand(
    Guid Id,
    decimal FinalScore,
    decimal? QualificationScore,
    CalculationMethod CalculationMethod) : IRequest<IResult<Unit>>;
