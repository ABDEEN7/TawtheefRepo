using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobResponsibility), Schema = Schemas.Hr)]
public class JobResponsibility : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    // Single responsibility line (Arabic)
    [MaxLength(1000)]
    public required string TextAr { get; set; }

    // Single responsibility line (English)
    [MaxLength(1000)]
    public required string TextEn { get; set; }

    public bool IsMandatory { get; set; }
}

