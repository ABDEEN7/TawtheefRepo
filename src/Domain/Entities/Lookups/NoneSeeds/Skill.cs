using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;

[Table(nameof(Skill), Schema = Schemas.Lookup)]
public class Skill : LookupBase
{
    public Guid? MajorId { get; set; }
    public Major? Major { get; set; }

    public Guid SkillTypeId { get; set; }
    public SkillType? SkillType { get; set; }

    public Guid SkillRequirementTypeId { get; set; }
    public SkillRequirementType? SkillRequirementType { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}
