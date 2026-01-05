using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Recruitment;

[Table(nameof(UserProfileLogger), Schema = Schemas.Hr)]
public class UserProfileLogger : EventEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }

    public Guid? PerformedById { get; set; }
    public User? PerformedBy { get; set; }

    [Required, StringLength(200)]
    public required string ActionType { get; set; }

    [StringLength(1024)]
    public string? Notes { get; set; }

    [StringLength(200)]
    public string? Section { get; set; }

    public Guid? EntityId { get; set; }

    public Guid? AttachmentId { get; set; }

    public ReviewStatus? ReviewStatus { get; set; }
}
