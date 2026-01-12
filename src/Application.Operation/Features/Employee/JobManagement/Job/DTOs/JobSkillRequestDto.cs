namespace Application.Operation.Features.Employee.Job.DTOs;

public class JobSkillRequestDto
{
    public required Guid SkillId { get; set; }
    public bool ShowToApplicants { get; set; } = false;
}
