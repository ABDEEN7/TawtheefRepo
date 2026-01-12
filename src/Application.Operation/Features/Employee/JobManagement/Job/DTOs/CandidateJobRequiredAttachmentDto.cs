namespace Application.Operation.Features.Employee.JobManagement.Job.DTOs;

public sealed class CandidateJobRequiredAttachmentDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string? Title { get; set; }
    public bool IsMandatory { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}
