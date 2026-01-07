using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobCandidateNationalityPercentage), Schema = Schemas.Hr)]
public class JobCandidateNationalityPercentage : EventEntity
{
    public Guid JobCandidateFilterSettingId { get; set; }
    public JobCandidateFilterSetting? JobCandidateFilterSetting { get; set; }

    public Guid CandidateTypeId { get; set; }
    public CandidateType? CandidateType { get; set; }

    public Guid NationalityId { get; set; }
    public Country? Nationality { get; set; }

    public int Percentage { get; set; }
}
