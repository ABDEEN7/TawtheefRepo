namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobConditionResponseDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public required string TextAr { get; set; }
    public required string TextEn { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
