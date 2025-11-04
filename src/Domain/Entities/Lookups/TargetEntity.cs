using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class TargetEntityIds
{
    public static readonly Guid Schools = Guid.Parse("1548322c-6c05-4754-a89c-19e9d2443d62");
    public static readonly Guid Ministry  = Guid.Parse("8fadf8df-ae7e-4d22-8cb8-f79ed9b14375");
}
/// <summary>
/// هي الجهة التي يرغب المرشح العمل فيها
/// </summary>
[Table(nameof(TargetEntity), Schema = Schemas.Lookup)]
public class TargetEntity: LookupBase
{
    
}
