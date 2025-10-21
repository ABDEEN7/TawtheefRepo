using System;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class GenderIds
{
    public static Guid Male = Guid.Parse("6720E352-B360-41F0-8D3B-FA5116B7A0B4");
    public static Guid Female = Guid.Parse("03CBE4E3-DC47-4D0C-8A07-87917AF1D2DD");
}
public class Gender : LookupBase
{
    
}
