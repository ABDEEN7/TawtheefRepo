using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class SkillTypeIds
{
    public static Guid Communication = Guid.Parse("77777777-0000-0000-0000-000000000001");
    public static Guid Leadership = Guid.Parse("77777777-0000-0000-0000-000000000002");
    public static Guid TimeManagement = Guid.Parse("77777777-0000-0000-0000-000000000003");
}
[Table(nameof(SkillType), Schema = "lkp")]
public class SkillType : LookupBase
{
}
