using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

[Table(nameof(City), Schema = Schemas.Lookup)]
public class City : LookupBase
{
    public Guid CountryId { get; set; }
    public Country? Country { get; set; }
    
    public int Code { get; set; }
}
