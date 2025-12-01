using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

[Table(nameof(Experience), Schema = Schemas.Profile)]
public class Experience : EventEntity
{
    public required string Organization { get; set; }
    public required string Position { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    
    public Guid CertificateId { get; set; }
    public Resource? Certificate { get; set; }
    
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
    /// <summary>
    /// list of achievements  separated by unique line '\n'
    /// </summary>
    public string? Achievements { get; set; }
}
