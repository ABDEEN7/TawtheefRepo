using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

public class ApplicantSkill : EventEntity
{
    public Guid SkillId { get; set; }
    public SkillType? Skill { get; set; }
    
    public Guid UserId { get; set; }
    public ApplicantUser? User { get; set; }
}
