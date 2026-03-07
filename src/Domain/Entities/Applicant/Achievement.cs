using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

[Table(nameof(Achievement), Schema = Schemas.Profile)]
[Index(nameof(UserProfileId))]
[Index(nameof(AttachmentId))]
public class Achievement : EventEntity
{
    public Guid UserProfileId { get; init; }
    public UserProfile? UserProfile { get; set; }

    public required Guid AchievementTypeId { get; set; }
    public AchievementType? AchievementType { get; init; }
    [MaxLength(64)]
    public required string Title { get; set; }
    [MaxLength(128)]
    public required string IssuingAuthority { get; set; }

    public required Guid CountryId { get; set; }
    public Country? Country { get; init; }

    public DateOnly IssueDate { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }

    public bool? RelatedToSpecialization { get; set; }

    public Guid AttachmentId { get; set; }
    public Resource? Attachment { get; init; }
}
