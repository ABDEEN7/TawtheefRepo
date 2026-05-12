using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

public static class InvitationExpiryConfigurationIds
{
    public static Guid Default = Guid.Parse("d3ecf2b4-8ddf-48d7-859d-88b7fe32ef13");
}

[Table(nameof(InvitationExpiryConfiguration), Schema = Schemas.Hr)]
public class InvitationExpiryConfiguration : EventEntity
{
    [Required]
    public int ExpiryDays { get; set; }
}
