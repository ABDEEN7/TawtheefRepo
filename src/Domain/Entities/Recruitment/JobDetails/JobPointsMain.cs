using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobPointsMain), Schema = Schemas.Hr)]
public class JobPointsMain : EventEntity
{
    [Required]
    public Guid JobId { get; set; }
    public virtual Job Job { get; set; } = default!;
    [Required, Range(0, 100)] public int ApplicantCategory { get; set; }
    [Required, Range(0, 100)] public int Education { get; set; }
    [Required, Range(0, 100)] public int Experience { get; set; }
    [Required, Range(0, 100)] public int Training { get; set; }
    [Required, Range(0, 100)] public int Certificates { get; set; }
    [Required, Range(0, 100)] public int Skills { get; set; }
    [Required, Range(0, 100)] public int Languages { get; set; }
    [Required, Range(0, 1000)] public int Total { get; set; }

    public virtual ICollection<JobPointsDetail>? Details { get; set; } = new List<JobPointsDetail>();
}
