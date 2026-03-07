using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobPointsMain), Schema = Schemas.Hr)]
[Index(nameof(JobId))]
public class JobPointsMain : EventEntity
{
    [Required]
    public Guid JobId { get; set; }
    public virtual Job Job { get; set; } = default!;
    [Required] public int ApplicantCategory { get; set; }
    [Required] public int Education { get; set; }
    [Required] public int Experience { get; set; }
    [Required] public int Training { get; set; }
    [Required] public int Certificates { get; set; }
    [Required] public int Skills { get; set; }
    [Required] public int Languages { get; set; }
    [Required, Range(0, 1000)] public int Total { get; set; }

    [Required]
    public bool IsApproved { get; set; } = false;

    public virtual ICollection<JobPointsDetail> Details { get; set; } = [];
}
