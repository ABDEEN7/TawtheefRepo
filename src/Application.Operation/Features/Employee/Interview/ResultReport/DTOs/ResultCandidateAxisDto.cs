namespace Application.Operation.Features.Employee.Interview.ResultReport.DTOs;

public sealed record ResultCandidateAxisDto(
    Guid AxisId,
    string? AxisNameAr,
    string? AxisNameEn,
    decimal Score,
    decimal? QualificationScore,
    bool? QualificationMet);
