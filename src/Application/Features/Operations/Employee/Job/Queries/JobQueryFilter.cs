namespace Tawtheef.Application.Features.Operations.Employee.Job.Queries;

public record JobQueryFilter
{
    public string? SearchTerm { get; init; }
    public Guid? DepartmentId { get; init; }
    public Guid? StatusId { get; init; }
    public Guid? JobCategoryId { get; init; }
    public Guid? WorkTypeId { get; init; }
    public DateTimeOffset? DeadlineFrom { get; init; }
    public DateTimeOffset? DeadlineTo { get; init; }
    public int? MinVacancies { get; init; }
    public int? MaxVacancies { get; init; }
}
