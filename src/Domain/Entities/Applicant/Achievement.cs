using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

[Table(nameof(Achievement), Schema = Schemas.Profile)]
public class Achievement : EventEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }

    public required Guid AchievementTypeId { get; set; }
    public AchievementType? AchievementType { get; set; }

    public required string Title { get; set; }
    public required string IssuingAuthority { get; set; }

    public required Guid CountryId { get; set; }
    public Country? Country { get; set; }

    public DateOnly IssueDate { get; set; }
    public string? Description { get; set; }

    public Guid AttachmentId { get; set; }
    public Resource? Attachment { get; set; }
}
