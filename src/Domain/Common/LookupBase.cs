using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Common.Interfaces;

namespace Tawtheef.Domain.Common;

public class LookupBase : EventEntity, ILocalizedName, ILocalizedDescription
{
    [MaxLength(50)]
    public required string BackendName { get; init; }
    [Required, MaxLength(200)] 
    public required string NameAr { get; set; }
    [Required, MaxLength(200)] 
    public required string NameEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public int DisplayOrder { get; init; } = 0;
}
