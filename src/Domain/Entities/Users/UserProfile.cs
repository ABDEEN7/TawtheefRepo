using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Users;

public class UserProfile : EventEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    [StringLength(2048)]
    public string? Bio { get; set; }
    
    public Guid? GenderId { get; set; }
    public Gender? Gender { get; init; }
}
