using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public sealed class JobTabReviewUpsertDto
{
    public TabType Tab { get; set; }
    public TabStatus? Status { get; set; }
    public string? Note { get; set; }
}
