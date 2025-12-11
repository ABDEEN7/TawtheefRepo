using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Recruitment;

[Table(nameof(ProfileReviewDecision), Schema = Schemas.Hr)]
public class ProfileReviewDecision : EventEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }

    public FinalApprovalAction Action { get; set; }
    public string? Notes { get; set; }
    public string? Summary { get; set; }

    public Guid? AttachmentResourceId { get; set; }
    public Resource? AttachmentResource { get; set; }

    public bool ExceptionalFlag { get; set; }
}
