using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class CountryIds
{
    public static Guid USA = Guid.Parse("12345678-1234-1234-1234-123456789012");
    public static Guid Canada = Guid.Parse("22345678-1234-1234-1234-123456789012");
}
[Table(nameof(Country), Schema = Schemas.Lookup)]
public class Country : LookupBase
{
    public int Code { get; set; }
}
