using System;

namespace Application.Operation.Features.Interview.EvaluationTemplate.DTOs;

public sealed record TemplateVersionCriterionDto(
    Guid Id,
    Guid InterviewTemplateEvaluationAxisId,
    Guid? InterviewEvaluationCriterionId,
    string? NameAr,
    string? NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    decimal MaxScore,
    bool IsRequired,
    int OrderNo,
    string? Notes);
