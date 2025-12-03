using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobResponsibility), Schema = Schemas.Hr)]
public class JobResponsibility : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    [MaxLength(500)]
    public required string Text { get; set; }
}
