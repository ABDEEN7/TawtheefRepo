namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobResponsibilityResponseDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string Text { get; set; } = string.Empty;
}
