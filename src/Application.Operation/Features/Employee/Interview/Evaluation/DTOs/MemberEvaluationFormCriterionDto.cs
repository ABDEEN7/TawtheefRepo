namespace Application.Operation.Features.Employee.Interview.Evaluation.DTOs;

public sealed record MemberEvaluationFormCriterionDto(
    Guid CriterionId,
    string? NameAr,
    string? NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    decimal MaxScore,
    bool IsRequired,
    int OrderNo,
    decimal? Score,
    string? Notes);
