namespace Application.Operation.Features.Employee.JobManagement.Job.DTOs;

public class JobConditionResponseDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public required string TextAr { get; set; }
    public required string TextEn { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}
