using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

[Table(nameof(Experience), Schema = Schemas.Profile)]
[Index(nameof(UserProfileId))]
[Index(nameof(QualificationId))]
[Index(nameof(CertificateId))]
public class Experience : EventEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
    [MaxLength(256)]
    public required string EmployerName { get; set; }
    [MaxLength(64)]
    public required string JobTitle { get; set; }

    public required Guid CountryId { get; set; }
    public Country? Country { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }

    public SpecializationRelationLevel? SpecializationRelation { get; set; }

    public Guid? QualificationId { get; set; }
    public Qualification? Qualification { get; set; }

    public Guid CertificateId { get; set; }
    public Resource? Certificate { get; set; }
}
