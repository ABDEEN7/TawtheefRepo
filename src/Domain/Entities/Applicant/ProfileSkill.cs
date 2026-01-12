using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

[Table(nameof(ProfileSkill), Schema = Schemas.Profile)]
//[Index(nameof(SkillId), nameof(UserProfileId), IsUnique = true)]
public class ProfileSkill : EventEntity
{
    public Guid SkillId { get; set; }
    public Skill? Skill { get; set; }

    public Guid LevelId { get; set; }
    public SkillLevel? Level { get; set; }

    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
}
