using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class NationalityIds
{
    public static Guid Qatar = Guid.Parse("22345678-1234-1234-1234-123456789012");
    public static Guid Saudi = Guid.Parse("12345678-1234-1234-1234-123456789012");
    public static Guid Jordan = Guid.Parse("32345678-1234-1234-1234-123456789012");
    public static Guid Egypt = Guid.Parse("42345678-1234-1234-1234-123456789012");
    public static Guid Sudan = Guid.Parse("52345678-1234-1234-1234-123456789012");
}
[Table(nameof(Nationality), Schema = Schemas.Lookup)]
public class Nationality : LookupBase
{
    
}
