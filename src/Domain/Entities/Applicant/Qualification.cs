using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

[Table(nameof(Qualification), Schema = Schemas.Profile)]
[Index(nameof(UserProfileId))]
[Index(nameof(MajorId))]
[Index(nameof(SubMajorId))]
[Index(nameof(UniversityId))]
[Index(nameof(StudyTypeId))]
[Index(nameof(RatingId))]
[Index(nameof(CertificateId))]
public class Qualification : EventEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
    
    public required Guid DegreeId { get; set; }
    public Degree? Degree { get; set; }
    public required Guid CountryId { get; set; }
    public Country? Country { get; set; }
    public Guid? MajorId { get; set; }
    public Major? Major { get; set; }
    public Guid? SubMajorId { get; set; }
    public Major? SubMajor { get; set; }
    public Guid? UniversityId { get; set; }
    public University? University { get; set; }
    public int? GraduationYear { get; set; }
    public Guid? StudyTypeId { get; set; }
    public StudyType? StudyType { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? GPA { get; set; }
    public Guid? RatingId { get; set; }
    public RatingGrade? Rating { get; set; }
    public Guid? CertificateId { get; set; }
    public Resource? Certificate { get; set; }
}
