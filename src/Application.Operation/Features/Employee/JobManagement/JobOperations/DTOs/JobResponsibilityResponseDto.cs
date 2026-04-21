namespace Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;

public class JobResponsibilityResponseDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public required string TextAr { get; set; }
    public required string TextEn { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}
