using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;
[Table(nameof(SkillType), Schema = Schemas.Lookup)]
public class SkillType : LookupBase
{
}
