namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;

public class CandidateEligibilityConditionDto
{
    public string Code { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "Passed", "Failed", "NotApplicable"
    public string Label { get; set; } = string.Empty;
    public string? ExpectedValue { get; set; }
    public string? ActualValue { get; set; }
    public string? FailureReason { get; set; }
    public object? ExpectedValueParams { get; set; }
    public object? ActualValueParams { get; set; }
    public object? FailureReasonParams { get; set; }
}
