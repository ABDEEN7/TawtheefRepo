using System;
using Tawtheef.Domain.Common.Interfaces;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Common;

public abstract class BaseEntity : IBaseEntity
{
    public Guid Id { get; set; }
    public Guid? CreatedById { get; set; }
    public User? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    
    public Guid? UpdatedById { get; set; }
    public User? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public Guid? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
    
    public bool IsDeleted { get; set; }
}
