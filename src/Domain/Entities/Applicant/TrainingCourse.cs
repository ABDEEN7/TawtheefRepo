using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

[Table(nameof(TrainingCourse), Schema = Schemas.Profile)]
[Index(nameof(UserProfileId))]
[Index(nameof(CertificateId))]
public class TrainingCourse : EventEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
    
    [MaxLength(128)]
    public required string Title { get; set; }
    [MaxLength(128)]
    public required string Provider { get; set; }
    public required Guid CountryId { get; set; }
    public Country? Country { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    [MaxLength(250)]
    public string? Description { get; set; }

    public SpecializationRelationLevel? SpecializationRelation { get; set; }

    public Guid CertificateId { get; set; }
    public Resource? Certificate { get; set; }
}
