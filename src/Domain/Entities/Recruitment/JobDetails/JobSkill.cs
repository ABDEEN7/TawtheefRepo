using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

public class JobSkill: EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }
    public int Order { get; set; }
    public required string Text { get; set; }
}
