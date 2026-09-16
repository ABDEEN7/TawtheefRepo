namespace Application.Operation.Features.Employee.Exams.DTOs;

public sealed record ExamLookupsDto(List<ExamLookupItemDto> Categories,
    List<ExamLookupItemDto> InterruptionPolicies, Guid SpecializedCategoryId);

public sealed record ExamLookupItemDto(Guid Id, string NameAr, string NameEn);
