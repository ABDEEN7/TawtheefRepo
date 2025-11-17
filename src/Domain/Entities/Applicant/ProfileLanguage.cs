using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

[Table(nameof(ProfileLanguage), Schema = Schemas.Profile)]
public class ProfileLanguage : EventEntity
{
    public Guid LanguageId { get; set; }
    public Language? Language { get; set; }
    
    public Guid LevelId { get; set; }
    public LanguageLevel? Level { get; set; }
    
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
}
