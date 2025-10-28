using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class StudyTypeIds
{
    public static Guid Regular = Guid.Parse("b28f6a3a-e3ec-401e-9bc7-c4156b4bf76f");
    public static Guid DistanceLearning = Guid.Parse("0a09774f-fa61-4896-b808-c8576cd0bf6b");
    public static Guid Affiliation = Guid.Parse("29754e1d-9125-4582-a998-68a72b8f8443");
}

[Table(nameof(StudyType), Schema = "lkp")]
public class StudyType : LookupBase
{
}
