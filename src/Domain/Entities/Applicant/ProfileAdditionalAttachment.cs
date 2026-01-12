using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

[Table(nameof(ProfileAdditionalAttachment), Schema = Schemas.Profile)]
public class ProfileAdditionalAttachment: EventEntity
{
    [MaxLength(128)]
    public required string FileName { get; set; }
    public Guid AttachmentId { get; set; }
    public Resource? Attachment { get; set; }
    
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
}
