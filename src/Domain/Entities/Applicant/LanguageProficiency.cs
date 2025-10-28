using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

public class LanguageProficiency : EventEntity
{
    public Guid LanguageId { get; set; }
    public Language? Language { get; set; }
    
    public Guid LevelId { get; set; }
    public LanguageLevel? Level { get; set; }
    
    public Guid UserId { get; set; }
    public ApplicantUser? User { get; set; }
}
