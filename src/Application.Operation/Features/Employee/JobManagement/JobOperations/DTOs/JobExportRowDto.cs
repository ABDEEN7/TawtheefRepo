namespace Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;

public sealed record JobExportRowDto(
    string TitleAr,
    string TitleEn,
    string JobCategoryAr,
    string JobCategoryEn,
    string GenderAr,
    string GenderEn,
    string StatusAr,
    string StatusEn,
    string SectorAr,
    string SectorEn,
    string ManagementAr,
    string ManagementEn,
    string DepartmentAr,
    string DepartmentEn,
    int NumberOfVacancies,
    DateTime ClosingDate,
    string CreatedByAr,
    string CreatedByEn,
    DateTime LastActionDate);
