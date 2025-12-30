using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;
[Table(nameof(University), Schema = Schemas.Lookup)]
public class University : LookupBase
{
    public Guid CityId { get; init; }
    public City? City { get; init; }
    
    public string? WebSite { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Code { get; init; }
    public string? LogoEn { get; init; }
    public string? LogoAr { get; init; }
    public string? OriginalName { get; init; }
}
