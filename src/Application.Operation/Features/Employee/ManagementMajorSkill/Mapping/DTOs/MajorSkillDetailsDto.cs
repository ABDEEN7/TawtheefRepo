using Application.Operation.Features.Employee.ManagementMajorSkill.Majors.DTOs;
using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.DTOs;

public record MajorSkillDetailsDto(
    Guid Id,
    Guid MajorId,
    MajorDetailsDto Major,
    Guid SkillId,
    DropdownOptions Skill,
    bool IsActive
);
