using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;
[Table(nameof(JobDegree), Schema = Schemas.Hr)]

[Index(nameof(JobId))]
[Index(nameof(DegreeId))]
public class JobDegree: EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public Guid DegreeId { get; set; }
    public Degree? Degree { get; set; }
}
