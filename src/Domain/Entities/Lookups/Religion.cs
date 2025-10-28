using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class ReligionIds
{
    public static Guid Islam     = Guid.Parse("5970a291-e636-4fd4-ab27-2af22dd77e9a");
    public static Guid Christian = Guid.Parse("51588ec8-2d50-4365-aa8c-84efaec02e09");
    public static Guid Hindu     = Guid.Parse("7d76cd4c-915a-4ce2-a2b0-decddf0f7490");
    public static Guid Buddhist  = Guid.Parse("c419dbdf-d0fc-4555-af2a-c5ed46847f8f");
    public static Guid Sikh      = Guid.Parse("6af60f76-d289-42d1-a3de-a3fa89056678");
    public static Guid Other     = Guid.Parse("185cbc18-2a59-40b0-a8d9-64b451f91ddf");
}
[Table(nameof(Religion), Schema = "lkp")]
public class Religion : LookupBase
{
    
}
