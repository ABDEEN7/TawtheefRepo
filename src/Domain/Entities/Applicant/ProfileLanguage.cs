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
    
    public Guid SpeakingLevelId { get; set; }
    public LanguageLevel? SpeakingLevel { get; set; }

    public Guid WritingLevelId { get; set; }
    public LanguageLevel? WritingLevel { get; set; }

    public Guid ReadingLevelId { get; set; }
    public LanguageLevel? ReadingLevel { get; set; }
    
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
}
