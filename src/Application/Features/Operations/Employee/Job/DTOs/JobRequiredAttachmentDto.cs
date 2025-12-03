namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobRequiredAttachmentDto
{
    public Guid? Id { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = default!;
    public bool IsMandatory { get; set; }
}
