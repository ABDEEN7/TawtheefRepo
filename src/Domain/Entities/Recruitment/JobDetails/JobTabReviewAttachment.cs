using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobTabReviewAttachment), Schema = Schemas.Hr)]
public class JobTabReviewAttachment : EventEntity
{
    public Guid JobTabReviewNoteId { get; set; }
    public JobTabReviewNote? JobTabReviewNote { get; set; }
    public required string FileName { get; set; } = default!;
    public Guid AttachmentId { get; set; }
    public Resource? Attachment { get; set; }
}
