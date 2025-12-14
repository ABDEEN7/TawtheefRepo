using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobTabReviewAttachment), Schema = Schemas.Hr)]
public class JobTabReviewAttachment : EventEntity
{
    [Required]
    public Guid JobTabReviewNoteId { get; set; }

    [ForeignKey(nameof(JobTabReviewNoteId))]
    public JobTabReviewNote? JobTabReviewNote { get; set; }

    [Required]
    public string FileName { get; set; } = default!;

    [Required]
    public Guid AttachmentId { get; set; }

    [ForeignKey(nameof(AttachmentId))]
    public Resource? Attachment { get; set; }
}
