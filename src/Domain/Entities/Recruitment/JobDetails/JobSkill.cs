using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobSkill), Schema = Schemas.Hr)]

public class JobSkill: EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }
    public Guid SkillId { get; set; }
    public Skill? Skill { get; set; }
    public bool ShowToApplicants { get; set; }
}
