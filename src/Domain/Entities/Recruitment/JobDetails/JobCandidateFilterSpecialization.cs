using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobCandidateFilterSpecialization), Schema = Schemas.Hr)]
public class JobCandidateFilterSpecialization : EventEntity
{
    public Guid JobCandidateFilterSettingId { get; set; }
    public JobCandidateFilterSetting? JobCandidateFilterSetting { get; set; }

    public Guid MajorId { get; set; }
    public Major? Major { get; set; }
    public Guid SubMajorId { get; set; }
    public Major? SubMajor { get; set; }
}
