namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;

public class CandidateEligibilityProfileSummaryDto
{
    public string Status { get; set; } = string.Empty;
    public bool AvailableForRecruitment { get; set; }
    public Guid? TargetEntityId { get; set; }
    public Guid? GenderId { get; set; }
    public DateOnly? BirthDate { get; set; }
    public List<Guid> QualificationLevelIds { get; set; } = new();
    public List<Guid> SkillIds { get; set; } = new();
}
