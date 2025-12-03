namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobConditionDto
{
    public Guid? Id { get; set; } 
    public int Order { get; set; }
    public string Text { get; set; } = default!;
}
