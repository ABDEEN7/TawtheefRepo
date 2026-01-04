using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobPointsDetailCopyDto
{
    public JobPointRuleType Type { get; set; }
    public required string Code { get; set; }
    public string? Name { get; set; }
    public int Points { get; set; }
    public Guid? ReferenceId { get; set; }
}
