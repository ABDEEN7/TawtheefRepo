using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

[Table(nameof(TrainingCourse), Schema = Schemas.Profile)]
public class TrainingCourse : EventEntity
{
    public required string Organization { get; set; }
    public required string Position { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    
    public Guid CertificateId { get; set; }
    public Resource? Certificate { get; set; }
    
    public Guid UserId { get; set; }
    public ApplicantUser? User { get; set; }
}
