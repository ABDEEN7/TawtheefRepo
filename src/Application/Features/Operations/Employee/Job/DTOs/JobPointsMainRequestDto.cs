using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobPointsMainRequestDto
{
    public Guid? Id { get; set; }
    [Required]
    public Guid JobId { get; set; }

    [Required, Range(0, 250)]
    public int ApplicantCategory { get; set; }
    [Required, Range(0, 200)]
    public int Education { get; set; }
    [Required, Range(0, 150)]
    public int Experience { get; set; }
    [Required, Range(0, 150)]
    public int Training { get; set; }
    [Required, Range(0, 150)]
    public int Skills { get; set; }
    [Required, Range(0, 150)]
    public int Languages { get; set; }
    [Required, Range(0, 1000)]
    public int Total { get; set; }

    public List<JobPointsDetailCreateDto> Details { get; set; } = new();
}
