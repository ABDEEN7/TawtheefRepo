using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class SkillTypeIds
{
    public static readonly Guid Educational = Guid.Parse("2f1b6ce7-cbc3-2b5c-b264-a02c6a87be1d");
    public static readonly Guid Technical = Guid.Parse("d14ac141-c057-16f5-ddd8-97d02f6a7c9b");
    public static readonly Guid Professional = Guid.Parse("83b504d0-25c1-5ca0-6757-df299869f002");
    public static readonly Guid Other = Guid.Parse("f91eb9e6-7a3f-76d6-1fe1-4443cecc5b9a");
}
[Table(nameof(SkillType), Schema = Schemas.Lookup)]
public class SkillType : LookupBase
{
}
