using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;

[Table(nameof(JobTitle), Schema = Schemas.Lookup)]
public class JobTitle : EventEntity
{
    [Required]
    [MaxLength(100)]
    public required string JobNumber { get; set; }

    [Required]
    [MaxLength(200)]
    public required string JobNameAr { get; set; }

    [Required]
    [MaxLength(200)]
    public required string JobNameEn { get; set; }

    public bool IsActive { get; set; } = true;
}
