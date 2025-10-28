using Tawtheef.Domain.Common.Interfaces;

namespace Tawtheef.Domain.Common;

public class LookupBase : EventEntity, ILocalizedName, ILocalizedDescription
{
    public required string BackendName { get; set; }
    public required string NameAr { get; set; }
    public required string NameEn { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
}
