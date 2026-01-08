using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobTabReviewNote), Schema = Schemas.Hr)]
[Index(nameof(JobId), nameof(Tab), nameof(ReviewCycleId), IsUnique = true)]
public class JobTabReviewNote : EventEntity
{
    public Guid JobId { get; set; }

    public virtual Job? Job { get; set; }

    [Column(TypeName = "nvarchar(50)")]
    public required TabType Tab { get; set; }

    [MaxLength(2000)]
    public string? Note { get; set; } 

    [Column(TypeName = "nvarchar(50)")]
    public required TabStatus TabStatus { get; set; }

    [Required]
    public bool IsResolved { get; set; } = false;
    [Required]
    public Guid ReviewCycleId { get; set; }
    public void UpdateNote(string? note, TabStatus tabStatus)
    {
        Note = note;
        TabStatus = tabStatus;
    }
}

public enum TabType
{
    BasicData,
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
