using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;
[Table(nameof(Country), Schema = Schemas.Lookup)]
public class Country : LookupBase
{
    public int Code { get; set; }
    public required string ISOCode { get; set; }
    public required string CodeAlpha { get; set; }
    public bool IsActive { get; set; } = true;
    
}
