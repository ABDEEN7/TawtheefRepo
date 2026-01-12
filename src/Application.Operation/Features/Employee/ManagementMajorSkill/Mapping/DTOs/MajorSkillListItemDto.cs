using Application.Operation.Features.Employee.ManagementMajorSkill.Majors.DTOs;
using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.DTOs;

public record MajorSkillListItemDto(
    Guid Id,
    MajorDetailsDto Major,
    DropdownOptions Skill,
    bool IsSkillRequired,
    bool IsActive
);
