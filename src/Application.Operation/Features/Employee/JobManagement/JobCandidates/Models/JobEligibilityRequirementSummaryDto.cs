namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;

public class JobEligibilityRequirementSummaryDto
{
    public Guid TargetEntityId { get; set; }
    public Guid? GenderId { get; set; }
    public int MaximumAge { get; set; }
    public int MinimumAge { get; set; }
    public List<Guid> QualificationLevelIds { get; set; } = new();
    public List<Guid> RequiredSkillIds { get; set; } = new();
}
