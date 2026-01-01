using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.DTOs;

public record MajorSkillListItemDto(
    Guid Id,
    MajorDetailsDto Major,
    DropdownOptions Skill,
    bool IsSkillRequired,
    bool IsActive
);
