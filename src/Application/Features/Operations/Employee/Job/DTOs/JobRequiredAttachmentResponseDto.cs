namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobRequiredAttachmentResponseDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }
}
