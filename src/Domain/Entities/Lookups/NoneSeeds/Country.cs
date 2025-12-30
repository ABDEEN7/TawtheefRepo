using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;
[Table(nameof(Country), Schema = Schemas.Lookup)]
public class Country : LookupBase
{
    public int Code { get; init; }
    public required string ISOCode { get; init; }
    public required string CodeAlpha { get; init; }
    
}
