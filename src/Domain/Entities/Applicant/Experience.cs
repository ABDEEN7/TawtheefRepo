using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

[Table(nameof(Experience), Schema = Schemas.Profile)]
public class Experience : EventEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
    
    public required string EmployerName { get; set; }
    public required string JobTitle { get; set; }

    public required Guid CountryId { get; set; }
    public Country? Country { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Description { get; set; }

    public Guid? QualificationId { get; set; }
    public Qualification? Qualification { get; set; }

    public Guid CertificateId { get; set; }
    public Resource? Certificate { get; set; }
}
