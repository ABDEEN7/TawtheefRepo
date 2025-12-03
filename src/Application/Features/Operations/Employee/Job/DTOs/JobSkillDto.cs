namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobSkillDto
{
    public Guid? Id { get; set; }       
    public Guid? SkillId { get; set; }
    public bool ShowToApplicants { get; set; }
}
