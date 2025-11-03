using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

[Table(nameof(Major), Schema = Schemas.Lookup)]
public class Major : LookupBase
{
    
}
