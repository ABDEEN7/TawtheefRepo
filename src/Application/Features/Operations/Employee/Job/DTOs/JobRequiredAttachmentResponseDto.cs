namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobRequiredAttachmentResponseDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public required string TitleAr { get; set; }
    public required string TitleEn { get; set; }
    public required bool IsMandatory { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}
