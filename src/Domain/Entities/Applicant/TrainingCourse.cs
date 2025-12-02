using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

[Table(nameof(TrainingCourse), Schema = Schemas.Profile)]
public class TrainingCourse : EventEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
    
    public required string Title { get; set; }
    public required string Provider { get; set; }
    public required Guid CountryId { get; set; }
    public Country? Country { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Description { get; set; }
    
    public Guid CertificateId { get; set; }
    public Resource? Certificate { get; set; }
}
