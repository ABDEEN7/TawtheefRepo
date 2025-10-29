using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class MajorIds
{
    public static Guid Education = Guid.Parse("10101010-0000-0000-0000-000000000001");
    public static Guid Mathematics = Guid.Parse("10101010-0000-0000-0000-000000000002");
    public static Guid ComputerScience = Guid.Parse("10101010-0000-0000-0000-000000000003");
    public static Guid BusinessAdministration = Guid.Parse("10101010-0000-0000-0000-000000000004");
    public static Guid Engineering = Guid.Parse("10101010-0000-0000-0000-000000000005");
}
[Table(nameof(Major), Schema = Schemas.Lookup)]
public class Major : LookupBase
{
}
