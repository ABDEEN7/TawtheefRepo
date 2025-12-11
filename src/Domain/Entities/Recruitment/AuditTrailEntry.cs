using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment;

[Table(nameof(AuditTrailEntry), Schema = Schemas.Hr)]
public class AuditTrailEntry : EventEntity
{
    public Guid UserProfileId { get; set; }
    public Guid UserId { get; set; }

    [Required, StringLength(200)]
    public required string ActionType { get; set; }

    [StringLength(1024)]
    public string? Notes { get; set; }

    [StringLength(200)]
    public string? Section { get; set; }

    public Guid? EntityId { get; set; }
    public Guid? AttachmentId { get; set; }
}
