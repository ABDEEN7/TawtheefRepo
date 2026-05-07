namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;

public class CandidateEligibilityResultDto
{
    public Guid JobId { get; set; }
    public Guid CandidateId { get; set; }

    public string CandidateName { get; set; } = string.Empty;
    public string NationalNumber { get; set; } = string.Empty;

    public bool IsEligible { get; set; }
    public bool AppearsInEligibleCandidatesQuery { get; set; }

    public List<CandidateEligibilityConditionDto> Conditions { get; set; } = new();

    public CandidateEligibilityProfileSummaryDto? Profile { get; set; }

    public JobEligibilityRequirementSummaryDto? JobRequirements { get; set; }

    public CandidatePointsBreakdownDto? PointsBreakdown { get; set; }
}
