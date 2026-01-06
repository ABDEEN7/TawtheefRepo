using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class GenderIds
{
    public static Guid All = Guid.Parse("4436a8ce-3f1e-4581-be3d-839c67127a9f");
    public static Guid Male = Guid.Parse("6720E352-B360-41F0-8D3B-FA5116B7A0B4");
    public static Guid Female = Guid.Parse("03CBE4E3-DC47-4D0C-8A07-87917AF1D2DD");
}
[Table(nameof(Gender), Schema = Schemas.Lookup)]
public class Gender : LookupBase;
