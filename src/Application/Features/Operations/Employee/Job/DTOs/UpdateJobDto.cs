using Tawtheef.Application.Features.Operations.Employee.Job.Dtos;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class UpdateJobDto : CreateJobDto
{
    public Guid Id { get; set; }
    public Guid JobStatusId { get; set; }

    public DateTime? PublishAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}
