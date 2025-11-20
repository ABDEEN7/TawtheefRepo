using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class SponsorTypeIds
{
    public static Guid Individual = Guid.Parse("5970a291-e636-4fd4-ab27-2af22dd77e9a");
    public static Guid Company = Guid.Parse("51588ec8-2d50-4365-aa8c-84efaec02e09");
}
[Table(nameof(SponsorType), Schema = Schemas.Lookup)]
public class SponsorType : LookupBase
{
    
}
