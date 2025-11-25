using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;
[Table("JobDegrees")]

public class JobDegree: EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public Guid DegreeId { get; set; }
    public Degree? Degree { get; set; }
}
