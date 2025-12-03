namespace Tawtheef.Application.Features.Operations.Employee.Job.Queries;

public record JobQueryFilter
{
    // -------------------------------------
    // Search
    // -------------------------------------
    public string? SearchTerm { get; init; }

    // -------------------------------------
    // Lookups
    // -------------------------------------
    public Guid? SectorId { get; init; }
    public Guid? ManagementId { get; init; }

    public Guid? DepartmentId { get; init; }              // Optional
    public Guid? RequestingDepartmentId { get; init; }    // Use this one for filtering jobs

    public Guid? StatusId { get; init; }
    public Guid? JobCategoryId { get; init; }
    public Guid? WorkTypeId { get; init; }
    public Guid? WorkLocationId { get; init; }

    public Guid? GenderId { get; init; }

    public Guid? MajorId { get; init; }
    public Guid? SubMajorId { get; init; }

    // -------------------------------------
    // Age & Experience
    // -------------------------------------
    public int? MinAge { get; init; }
    public int? MaxAge { get; init; }

    public int? MinExperienceYears { get; init; }

    // -------------------------------------
    // Vacancies
    // -------------------------------------
    public int? MinVacancies { get; init; }
    public int? MaxVacancies { get; init; }

    // -------------------------------------
    // Deadline
    // -------------------------------------
    public DateTimeOffset? DeadlineFrom { get; init; }
    public DateTimeOffset? DeadlineTo { get; init; }

    // -------------------------------------
    // Publish date
    // -------------------------------------
    public DateTimeOffset? PublishFrom { get; init; }
    public DateTimeOffset? PublishTo { get; init; }
}
