using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.DTOs;

public record SkillDetailsDto : DropdownOptions
{
    public Guid SkillTypeId { get; init; }
    public DropdownOptions? SkillType { get; init; }

    public bool IsActive { get; init; }

    public int UsedInMappingsCount { get; set; }
}
