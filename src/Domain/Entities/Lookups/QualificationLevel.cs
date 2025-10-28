using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class QualificationLevelIds
{
    public static Guid Diploma = Guid.Parse("d3a7fa85-6e1a-489d-9606-1ab786d46066");
    public static Guid Bachelor = Guid.Parse("39ffab2f-86d1-4db3-9210-113a4fcd77e8");
    public static Guid Master = Guid.Parse("7137f1ec-1c46-414a-9071-dc40e984cc16");
    public static Guid Doctorate = Guid.Parse("a99f5bdc-a1a2-46ca-9211-0c85bddd1f75");
    public static Guid HighSchool = Guid.Parse("78292166-dff6-416f-ab04-0547331cbfee");
}

[Table(nameof(QualificationLevel), Schema = "lkp")]
public class QualificationLevel : LookupBase
{
}
