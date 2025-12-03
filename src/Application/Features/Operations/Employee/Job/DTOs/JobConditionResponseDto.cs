namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobConditionResponseDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string Text { get; set; } = string.Empty;
}
