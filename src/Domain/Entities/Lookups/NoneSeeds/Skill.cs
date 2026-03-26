using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;

[Table(nameof(Skill), Schema = Schemas.Lookup)]
[Index(nameof(SkillTypeId))]
public class Skill : LookupBase
{
    public Guid SkillTypeId { get; set; }
    public SkillType? SkillType { get; init; }
    
    public bool IsGeneral { get; set; }
    
    public void ChangeActivation(bool isActive)
    {
        IsActive = isActive;
    }
    
    public void UpdateDetails(string nameAr, string nameEn, string? descriptionAr, string? descriptionEn, Guid skillTypeId, bool isActive, bool isGeneral)
    {
        NameAr = nameAr;
        NameEn = nameEn;
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
        SkillTypeId = skillTypeId;
        IsActive = isActive;
        IsGeneral = isGeneral;
    }
}
