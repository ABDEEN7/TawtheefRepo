using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Domain.Common.Interfaces;

namespace Tawtheef.Domain.Entities.Users;

public class UserLogin : IdentityUserLogin<Guid>, IBaseEntity
{
    public Guid Id { get; set; }
    
    [MaxLength(1000)]
    public string? RawClaimsJson { get; set; }
    
    public Guid? CreatedById { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? UpdatedById { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? DeletedById { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
}
