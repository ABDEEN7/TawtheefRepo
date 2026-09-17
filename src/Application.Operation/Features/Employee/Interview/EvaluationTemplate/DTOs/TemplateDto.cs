using System;
using Tawtheef.Domain.Entities.Interview;

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
   bool IsActive,
    // Summary of the template's latest version (by VersionNo), null when no version has been created yet
    int? LatestVersionNo,
    TemplateVersionStatus? LatestVersionStatus,
    decimal? LatestVersionFinalScore,
    decimal? LatestVersionQualificationScore);
