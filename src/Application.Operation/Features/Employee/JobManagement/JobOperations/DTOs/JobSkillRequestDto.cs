namespace Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;

public class JobSkillRequestDto
{
    public required Guid SkillId { get; set; }
    public bool ShowToApplicants { get; set; } = false;
    public bool IsRequired { get; set; } = false;
}
