using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Domain.Entities.Recruitment;

[Table(nameof(InvitationAttachment), Schema = Schemas.Hr)]
[Index(nameof(InvitationId), nameof(JobRequiredAttachmentId), IsUnique = true)]
public class InvitationAttachment : EventEntity
{
    public Guid InvitationId { get; set; }
    public virtual Invitation? Invitation { get; set; }

    public Guid JobRequiredAttachmentId { get; set; }
    public virtual JobRequiredAttachment? JobRequiredAttachment { get; set; }

    public Guid ResourceId { get; set; }
    public virtual Resource? Resource { get; set; }

    [MaxLength(200)]
    public string AttachmentTitleEn { get; set; } = string.Empty;

    [MaxLength(200)]
    public string AttachmentTitleAr { get; set; } = string.Empty;

    public bool IsApproved { get; set; }
    
    public bool IsReturned { get; set; }

    [MaxLength(1000)]
    public string? ReviewNote { get; set; }
}
