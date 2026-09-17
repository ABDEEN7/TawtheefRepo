using System;
using System.Collections.Generic;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.DTOs;

public sealed record TemplateVersionDetailsDto(
    Guid Id,
    Guid InterviewTemplateId,
    int VersionNo,
    decimal FinalScore,
    decimal? QualificationScore,
    CalculationMethod CalculationMethod,
    TemplateVersionStatus Status,
    bool IsLocked,
    DateTime? EffectiveFrom,
    Guid? ApprovedById,
    DateTime? ApprovedAt,
    string? DecisionNotes,
    List<TemplateVersionAxisDto> Axes);
