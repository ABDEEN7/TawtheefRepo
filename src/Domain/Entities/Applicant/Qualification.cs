using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

[Table(nameof(Qualification), Schema = Schemas.Profile)]
public class Qualification : EventEntity
{
    public Guid LevelId { get; set; }
    public QualificationLevel? Level { get; set; }
    public Guid MajorId { get; set; }
    public Major? Major { get; set; }
    public Guid UniversityId { get; set; }
    public University? University { get; set; }
    public  int? GraduationYear { get; set; }
    public Guid StudyTypeId { get; set; }
    public StudyType? StudyType { get; set; }
    public required string GPA { get; set; }
    public Guid RatingId { get; set; }
    public RatingGrade? Rating { get; set; }
    public Guid CountryId { get; set; }
    public Country? Country { get; set; }
    public Guid CertificateId { get; set; }
    public Resource? Certificate { get; set; }

    public Guid UserId { get; set; }
    public ApplicantUser? User { get; set; }
}
