namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobSkillRequestDto
{
    public required Guid SkillId { get; set; }
    public bool ShowToApplicants { get; set; } = false;
}
