using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobManagement.Job.DTOs;

public class JobPointsDetailCreateDto
{
    [Required]
    public JobPointRuleType Type { get; set; }

    [Required]
    public required string Code { get; set; }

    public string? Name { get; set; }

    [Required]
    public int Points { get; set; }

    public Guid? ReferenceId { get; set; }
}
