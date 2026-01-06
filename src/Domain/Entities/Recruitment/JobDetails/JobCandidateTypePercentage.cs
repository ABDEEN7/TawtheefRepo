using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobCandidateTypePercentage), Schema = Schemas.Hr)]
public class JobCandidateTypePercentage : EventEntity
{
    public Guid JobCandidateFilterSettingId { get; set; }
    public JobCandidateFilterSetting? JobCandidateFilterSetting { get; set; }

    public Guid CandidateTypeId { get; set; }
    public CandidateType? CandidateType { get; set; }

    public int Percentage { get; set; }
}
