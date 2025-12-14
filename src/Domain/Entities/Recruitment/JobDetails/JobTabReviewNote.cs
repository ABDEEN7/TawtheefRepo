using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobTabReviewNote), Schema = Schemas.Hr)]
[Index(nameof(JobId), nameof(Tab), IsUnique = true)]
public class JobTabReviewNote : EventEntity
{
    [Required]
    public Guid JobId { get; set; }

    [ForeignKey(nameof(JobId))]
    public virtual Job? Job { get; set; }

    [Required]
    public required TabType Tab { get; set; }

    [MaxLength(2000)]
    public string? Note { get; set; } 

    [Required]
    public required TabStatus TabStatus { get; set; }

    public virtual ICollection<JobTabReviewAttachment> Attachments { get; set; } = new List<JobTabReviewAttachment>();

    public void UpdateNote(string? note, TabStatus tabStatus)
    {
        Note = note;
        TabStatus = tabStatus;
    }
}



public enum TabType
{
    Overview,
    Responsibilities,
    Qualifications,
    Conditions,
    Skills,
    Benefits,
    Attachments
}

public enum TabStatus
{
    Approved,
    Returned,
}
