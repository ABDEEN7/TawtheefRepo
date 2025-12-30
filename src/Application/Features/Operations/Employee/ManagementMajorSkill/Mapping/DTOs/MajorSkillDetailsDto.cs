using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.DTOs;

public record MajorSkillDetailsDto(
    Guid Id,
    Guid MajorId,
    DropdownOptions Major,
    Guid SkillId,
    DropdownOptions Skill,
    bool IsSkillRequired,
    bool IsActive
);
