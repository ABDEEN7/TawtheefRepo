using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewApprovalAction), Schema = Schemas.Interview)]
[Index(nameof(EntityType), nameof(EntityId))]
public class InterviewApprovalAction : EventEntity
{
    [Column(TypeName = "varchar(60)")]
    public required string EntityType { get; set; }

    public Guid EntityId { get; set; }

    public ApprovalAction Action { get; set; }

    public byte Level { get; set; }

    public string? Reason { get; set; }
}

public enum ApprovalAction
{
    Submit = 1,
    Approve = 2,
    Return = 3,
    Cancel = 4,
    Reopen = 5
}
