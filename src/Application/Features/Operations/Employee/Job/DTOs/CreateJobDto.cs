namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public record CreateJobDto
{
    public JobBasicsDto Basics { get; init; } = default!;
    public JobQuotasDto Quotas { get; init; } = default!;
    public string? Description { get; init; }
    public string? Benefits { get; init; }
    public List<string> Conditions { get; init; } = [];
    public List<string> Skills { get; init; } = [];
    public List<Guid> DegreeIds { get; init; } = [];
}
