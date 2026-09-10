using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Commands;

// Either InterviewEvaluationCriterionId is set (criterion picked from the evaluation bank, no name override),
// or it is null and NameAr is required (a custom criterion that only exists for this template version).
public sealed record AddTemplateVersionCriterionCommand(
    Guid InterviewTemplateEvaluationAxisId,
    Guid? InterviewEvaluationCriterionId,
    string? NameAr,
    string? NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    decimal MaxScore,
    bool IsRequired,
    int OrderNo,
    string? Notes) : IRequest<IResult<Guid>>;
