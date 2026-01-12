using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobCandidateFilterSetting), Schema = Schemas.Hr)]
public class JobCandidateFilterSetting : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public Guid? GenderId { get; set; }
    public Gender? Gender { get; set; }

    public int? MinimumPoints { get; set; }

    public ICollection<JobCandidateTypePercentage> CandidateTypePercentages { get; set; } = [];
    public ICollection<JobCandidateNationalityPercentage> NationalityPercentages { get; set; } = [];
}
