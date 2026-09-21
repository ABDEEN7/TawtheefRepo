using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.DTOs;

public sealed record TemplateLookupItemDto(Guid Id, string NameAr, string? NameEn);
public sealed record TemplateLookupsDto(
    List<TemplateLookupItemDto> OrganizationScopes,
    List<TemplateLookupItemDto> JobTitles,
    List<TemplateLookupItemDto> Departments);

