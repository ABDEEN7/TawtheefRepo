using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobReviewAttachment), Schema = Schemas.Hr)]
[Index(nameof(JobId),nameof(AttachmentId),nameof(ReviewCycleId), IsUnique = true)]
public class JobReviewAttachment : EventEntity
{
    [Required]
    public Guid JobId { get; set; }
    public virtual Job? Job { get; set; }
    public required string FileName { get; set; }
    [Required]
    public Guid ReviewCycleId { get; set; }
    [Required]
    public Guid AttachmentId { get; set; }
    public Resource? Attachment { get; set; }
}
