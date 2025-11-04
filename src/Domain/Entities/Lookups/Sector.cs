using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class SectorIds
{
    public static readonly Guid Schools = Guid.Parse("1548322c-6c05-4754-a89c-19e9d2443d62");
    public static readonly Guid Ministry  = Guid.Parse("8fadf8df-ae7e-4d22-8cb8-f79ed9b14375");
}
/// <summary>
/// هو المكان الذي سوف يعمل فيه المرشح ويتم تحديده داخل مرحلة انشاء بيانات الوظيفه
/// </summary>
[Table(nameof(Sector), Schema = Schemas.Lookup)]
public class Sector : LookupBase
{
    
}
