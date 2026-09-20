using System;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.DTOs;

public sealed record TemplateVersionDto(
    Guid Id,
    Guid InterviewTemplateId,
    int VersionNo,
    decimal FinalScore,
    decimal? QualificationScore,
    CalculationMethod CalculationMethod,
    TemplateVersionStatus Status,
    bool IsLocked,
    DateTimeOffset? EffectiveFrom,
    Guid? ApprovedById,
    DateTimeOffset? ApprovedAt,
    string? DecisionNotes);
