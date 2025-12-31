using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;

[Table(nameof(City), Schema = Schemas.Lookup)]
public class City : LookupBase
{
    public Guid CountryId { get; init; }
    public Country? Country { get; init; }
    
    public required string Code { get; init; }
}
