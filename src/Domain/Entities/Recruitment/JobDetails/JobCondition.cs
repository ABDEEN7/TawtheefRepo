using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobCondition), Schema = Schemas.Hr)]
public class JobCondition : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    // Single condition line (Arabic)
    [MaxLength(1000)]
    public required string TextAr { get; set; }

    // Single condition line (English)
    [MaxLength(1000)]
    public required string TextEn { get; set; }
}

