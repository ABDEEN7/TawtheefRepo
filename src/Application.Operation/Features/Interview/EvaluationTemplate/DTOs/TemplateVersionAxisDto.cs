using System;
using System.Collections.Generic;

namespace Application.Operation.Features.Interview.EvaluationTemplate.DTOs;

public sealed record TemplateVersionAxisDto(
    Guid Id,
    Guid InterviewTemplateVersionId,
    Guid InterviewEvaluationAxisId,
    string AxisNameAr,
    string? AxisNameEn,
    decimal MaxScore,
    decimal? QualificationScore,
    int OrderNo,
    List<TemplateVersionCriterionDto> Criteria);
