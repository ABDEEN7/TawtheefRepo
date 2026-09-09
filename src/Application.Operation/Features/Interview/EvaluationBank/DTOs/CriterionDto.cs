using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Operation.Features.Interview.EvaluationBank.DTOs;

public sealed record CriterionDto(
    Guid Id,
    Guid InterviewEvaluationAxisId,
    string NameAr,
    string? NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive);
