using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobSkill), Schema = Schemas.Hr)]

public class JobSkill: EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }
    public int Order { get; set; }
    [MaxLength(100)]
    public required string Text { get; set; }
}
