using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobCondition), Schema = Schemas.Hr)]
public class JobCondition: EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    [MaxLength(100)]
    public required string Text { get; set; }
}
