namespace Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;

public sealed class CandidateJobConditionDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string? Text { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}
