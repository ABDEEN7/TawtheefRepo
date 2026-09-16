namespace Application.Operation.Features.Employee.Exams.DTOs;

public sealed record ExamJobDto(Guid Id, string NameAr, string NameEn, string JobNumber,
    string? ManagementAr, string? ManagementEn, string? DepartmentAr, string? DepartmentEn,
    List<string> SpecializationsAr, List<string> SpecializationsEn);
