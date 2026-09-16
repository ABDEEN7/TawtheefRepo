using System;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.DTOs;

public sealed record TemplateDto(
    Guid Id,
    string TitleAr,
    string? TitleEn,
    Guid? OrganizationScopeId,
    string? OrganizationScopeNameAr,
    string? OrganizationScopeNameEn,
    Guid? JobTitleId,
    string? JobTitleNameAr,
    string? JobTitleNameEn,
    Guid? DepartmentId,
    string? DepartmentNameAr,
    string? DepartmentNameEn,
    bool IsActive);
