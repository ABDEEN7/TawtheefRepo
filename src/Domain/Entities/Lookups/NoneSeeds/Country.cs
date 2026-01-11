using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;

public static class CountryIds
{
    public static readonly Guid Qatar = Guid.Parse("42b88098-d82c-4dec-9ec9-3222c43f464c");
}
[Table(nameof(Country), Schema = Schemas.Lookup)]
public class Country : LookupBase
{
    public int Code { get; init; }
    public required string ISOCode { get; init; }
    public required string CodeAlpha { get; init; }
    
}
