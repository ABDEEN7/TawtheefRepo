using Tawtheef.Application.Features.Operations.Employee.Job.Dtos;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class UpdateJobDto : CreateJobDto
{
    public Guid Id { get; set; }
}
