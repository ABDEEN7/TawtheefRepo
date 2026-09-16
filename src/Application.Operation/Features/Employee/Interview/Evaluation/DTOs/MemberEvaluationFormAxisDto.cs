namespace Application.Operation.Features.Employee.Interview.Evaluation.DTOs;

public sealed record MemberEvaluationFormAxisDto(
    Guid AxisId,
    string? AxisNameAr,
    string? AxisNameEn,
    decimal MaxScore,
    int OrderNo,
    List<MemberEvaluationFormCriterionDto> Criteria);
