using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

public class JobDegree: EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public int DegreeId { get; set; }
    public Degree? Degree { get; set; }
}
