using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Users;

[Table(nameof(SponsorProfile), Schema = Schemas.Applicant)]
public class SponsorProfile : EventEntity
{
    public Guid SponsorTypeId { get; set; }
    public SponsorType? SponsorType { get; set; }
    
    [MaxLength(400)]
    public required string SponsorName { get; set; }
    [MaxLength(50)]
    public required string SponsorNumber { get; set; }
    
    
    public DateOnly? QIDExpiry { get; set; }
    
    public Guid? SponsorCardId { get; set; }
    public Resource? SponsorCard { get; set; }
}
