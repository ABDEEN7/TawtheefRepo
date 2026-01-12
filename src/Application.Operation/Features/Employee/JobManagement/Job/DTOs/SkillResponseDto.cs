using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.Job.DTOs;

public class SkillResponseDto
{
    public DropdownOptions Skill { get; set; } = new();
    public bool IsEssential { get; set; }

    public DropdownOptions Major { get; set; } = new();
    public DropdownOptions SkillType { get; set; } = new();
    public DropdownOptions SkillRequirementType { get; set; } = new();
}
