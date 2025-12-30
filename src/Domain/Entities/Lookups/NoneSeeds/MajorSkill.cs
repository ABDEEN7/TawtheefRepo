using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;

[Table(nameof(MajorSkill), Schema = Schemas.Hr)]
public class MajorSkill : EventEntity
{
    public Guid MajorId { get; init; }
    public Major? Major { get; init; }
    
    public Guid SkillId { get; init; }
    public Skill? Skill { get; init; }

    public bool IsSkillRequired { get; set; } = false;
    public bool IsActive { get; set; } = true;
}
