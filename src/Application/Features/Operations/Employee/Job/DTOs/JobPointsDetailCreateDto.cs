using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobPointsDetailCreateDto
{
    [Required]
    public JobPointRuleType Type { get; set; }

    [Required]
    public string Code { get; set; } = default!;

    public string? Name { get; set; }

    [Required]
    [Range(0, 500)]
    public int Points { get; set; }

    public Guid? ReferenceId { get; set; }
}
