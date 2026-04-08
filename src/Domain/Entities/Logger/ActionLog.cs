using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Logger;

[Table(nameof(ActionLog), Schema = Schemas.Hr)]
[Index(nameof(UserProfileId))]
[Index(nameof(UserId))]
[Index(nameof(EntityId))]
[Index(nameof(AttachmentId))]
public class ActionLog : EventEntity
{
    public Guid? UserProfileId { get; set; }
    public Guid? UserId { get; set; }

    [Required, StringLength(200)]
    public required string ActionType { get; set; }

    [Required]
    public ActionLogType LogType { get; set; }
 
    public string? Notes { get; set; }
 
    [StringLength(200)]
    public string? Section { get; set; }

    public Guid? EntityId { get; set; }
    public Guid? AttachmentId { get; set; }
}

public enum ActionLogType
{
    Admin = 1,
    Employee = 2
}

