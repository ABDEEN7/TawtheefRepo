namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public record JobBasicsDto
{
    public string Title { get; init; } = string.Empty;
    public Guid RequestingDeptId { get; init; }
    public Guid JobCategoryId { get; init; }
    public Guid GenderId { get; init; }
    public Guid TargetEntityId { get; init; }
    public Guid MajorId { get; init; }
    public Guid WorkTypeId { get; init; }
    public int Vacancies { get; init; }
    public DateTimeOffset Deadline { get; init; }
}
