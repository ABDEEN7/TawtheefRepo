using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class SectorIds
{
    public static readonly Guid DeputyMinisterSector = Guid.Parse("e1a7f8d9-3b4c-4a2d-8e5f-6a7b8c9d0e1f");
    public static readonly Guid GeneralEducationSector = Guid.Parse("f2b8e9fa-4c5d-5b3e-9f6a-7b8c9d0e1a2b");
    public static readonly Guid PrivateEducationSector = Guid.Parse("a3c9f0eb-5d6e-6c4f-0a7b-8c9d0e1a2b3c");
    public static readonly Guid AssessmentSector = Guid.Parse("b4da01fc-6e7f-7d50-1b8c-9d0e1a2b3c4d");
    public static readonly Guid SharedServicesSector = Guid.Parse("c5eb12fd-7f80-8e61-2c9d-0e1a2b3c4d5e");
}
/// <summary>
/// هو المكان الذي سوف يعمل فيه المرشح ويتم تحديده داخل مرحلة انشاء بيانات الوظيفه
/// </summary>
[Table(nameof(Sector), Schema = Schemas.Lookup)]
public class Sector : LookupBase
{
    
}
