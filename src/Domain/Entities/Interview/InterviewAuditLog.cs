using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewAuditLog), Schema = Schemas.Interview)]
[Index(nameof(EntityType), nameof(EntityId))]
public class InterviewAuditLog : EventEntity
{
    [Column(TypeName = "varchar(60)")]
    public required string EntityType { get; set; }

    public Guid EntityId { get; set; }

    [Column(TypeName = "varchar(60)")]
    public required string Action { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? Reason { get; set; }
}
