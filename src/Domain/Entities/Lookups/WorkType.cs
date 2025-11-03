using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class WorkTypeIds
{
    public static Guid FullTime = Guid.Parse("0fdfadbf-e5a0-41d3-a74d-d86354fed434");
    public static Guid PartTime = Guid.Parse("e30074cd-52c6-41b5-a692-57626d109c73");
}
[Table(nameof(WorkType), Schema = Schemas.Lookup)]
public class WorkType : LookupBase
{
    
}
