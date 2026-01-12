using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Recruitment.Features.JobDetails.DTOs;

public class JobPointsDetailResponseDto
{
    public Guid Id { get; set; }
    public JobPointRuleType Type { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int Points { get; set; }
    public Guid? ReferenceId { get; set; }
}
