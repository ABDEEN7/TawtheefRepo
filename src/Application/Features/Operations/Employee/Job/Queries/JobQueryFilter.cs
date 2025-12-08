namespace Tawtheef.Application.Features.Operations.Employee.Job.Queries;

public record JobQueryFilter
{
    public string? SearchTerm { get; init; }
    public Guid? SectorId { get; init; }
    public Guid? ManagementId { get; init; }
    public Guid? DepartmentId { get; init; }             
    public Guid? StatusId { get; init; }
    public Guid? JobCategoryId { get; init; }
    public Guid? WorkTypeId { get; init; }
    public Guid? WorkLocationId { get; init; }
    public Guid? GenderId { get; init; }
    public Guid? MajorId { get; init; }
    public Guid? SubMajorId { get; init; }
    public int? MinAge { get; init; }
    public int? MaxAge { get; init; }
    public int? MinExperienceYears { get; init; }
    public int? MinVacancies { get; init; }
    public int? MaxVacancies { get; init; }
    public DateTimeOffset? CloseDateFrom { get; init; }
    public DateTimeOffset? CloseDateTo { get; init; }
    public DateTimeOffset? PublishFrom { get; init; }
    public DateTimeOffset? PublishTo { get; init; }
}
