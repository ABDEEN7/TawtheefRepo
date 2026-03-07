using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Recruitment;

[Table(nameof(ProfileReviewDecision), Schema = Schemas.Hr)]
[Index(nameof(UserProfileId))]
[Index(nameof(AttachmentResourceId))]
public class ProfileReviewDecision : EventEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }

    public FinalApprovalAction Action { get; set; }

    // Detailed reviewer notes / justification
    [MaxLength(2000)]
    public string? Notes { get; set; }

    // Short decision summary (shown in lists / dashboards)
    [MaxLength(500)]
    public string? Summary { get; set; }

    public Guid? AttachmentResourceId { get; set; }
    public Resource? AttachmentResource { get; set; }

    public bool ExceptionalFlag { get; set; }
}

