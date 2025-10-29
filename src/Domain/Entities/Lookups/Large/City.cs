using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class CityIds
{
    public static Guid Doha = Guid.Parse("12345678-1234-1234-1234-123456789012");
    public static Guid AlSkhama = Guid.Parse("22345678-1234-1234-1234-123456789012");
}
[Table(nameof(City), Schema = Schemas.Lookup)]
public class City : LookupBase
{
    public int Code { get; set; }
}
