using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

public class JobCondition: EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }
    public int Order { get; set; }
    [MaxLength(100)]
    public required string Text { get; set; }
}
