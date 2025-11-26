namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public record UpdateJobDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public Guid RequestingDepartmentId { get; init; }
    public Guid JobCategoryId { get; init; }
    public Guid GenderId { get; init; }
    public Guid WorkLocationId { get; init; }
    public Guid MajorId { get; init; }
    public Guid WorkTypeId { get; init; }
    public int Vacancies { get; init; }
    public DateTimeOffset Deadline { get; init; }
    public required JobQuotaDto Quota { get; init; }
    public required string Description { get; init; }
    public required string Benefits { get; init; }
    public List<string> Conditions { get; init; } = [];
    public List<string> Skills { get; init; } = [];
    public List<Guid> DegreeIds { get; init; } = [];
    
    public Guid StatusId { get; init; }

}
