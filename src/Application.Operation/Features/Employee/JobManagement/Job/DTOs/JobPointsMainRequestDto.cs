using System.ComponentModel.DataAnnotations;

namespace Application.Operation.Features.Employee.JobManagement.Job.DTOs;

public class JobPointsMainRequestDto
{
    public Guid? Id { get; set; }
    [Required]
    public Guid JobId { get; set; }

    [Required]
    public int ApplicantCategory { get; set; }
    [Required]
    public int Education { get; set; }
    [Required]
    public int Experience { get; set; }
    [Required]
    public int Training { get; set; }
    [Required]
    public int Skills { get; set; }
    [Required]
    public int Languages { get; set; }
    [Required]
    public int Certificates { get; set; }
    [Required, Range(0, 1000)]
    public int Total { get; set; }

    public List<JobPointsDetailCreateDto> Details { get; set; } = new();
}
