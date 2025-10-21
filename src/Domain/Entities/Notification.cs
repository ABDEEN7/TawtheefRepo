using System;
using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities;

public class Notification : EventEntity
{
    [Required, StringLength(200)]
    public required string Title { get; init; }
    [Required, StringLength(1000)]
    public required string Message { get; init; }
    public bool IsRead { get; init; }
    public DateTime? ScheduledSendDate { get; init; }
    
    public Guid TypeId { get; init; }
    public virtual NotificationType? Type { get; init; }
    
    public Guid UserId { get; init; }
    public virtual User? User { get; init; }
    
    public Guid? RelatedEntityId { get; init; }
}
