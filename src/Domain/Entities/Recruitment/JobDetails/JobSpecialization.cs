using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobSpecialization), Schema = Schemas.Hr)]
public class JobSpecialization : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public Guid MajorId { get; set; }
    public Major? Major { get; set; }

    public Guid SubMajorId { get; set; }
    public Major? SubMajor { get; set; }
}
