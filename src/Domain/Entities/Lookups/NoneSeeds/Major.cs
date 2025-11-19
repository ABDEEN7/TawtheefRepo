using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;

[Table(nameof(Major), Schema = Schemas.Lookup)]
public class Major : LookupBase
{
    public Guid? ParentId { get; set; }
    public Major? Parent { get; set; }
}
