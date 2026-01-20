using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common.Interfaces;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Common;

//TODO: check how re-order columns in table
public abstract class BaseEntity : IBaseEntity
{
    public Guid Id { get; set; }
    
    [Column(Order = 93)]
    public Guid? CreatedById { get; set; }
    public User? CreatedBy { get; set; }
    
    [Column(Order = 94)]
    public DateTimeOffset CreatedDate { get; set; }
    
    [Column(Order = 95)]
    public Guid? UpdatedById { get; set; }
    public User? UpdatedBy { get; set; }
    
    [Column(Order = 96)]
    public DateTimeOffset? UpdatedDate { get; set; }
    
    [Column(Order = 97)]
    public Guid? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
    
    [Column(Order = 98)]
    public DateTimeOffset? DeletedDate { get; set; }
    [Column(Order = 99)]
    public bool IsDeleted { get; set; }
}
