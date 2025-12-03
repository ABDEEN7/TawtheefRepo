namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobSkillResponseDto
{
    public Guid Id { get; set; }
    public SkillResponseDto? Skill { get; set; }
    public bool ShowToApplicants { get; set; }
}
