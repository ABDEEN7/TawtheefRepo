using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class CandidateTypeIds
{
    public static readonly Guid Qatari = Guid.Parse("268d49f6-0dda-4dd6-8346-be355c553496");
    public static readonly Guid GCC  = Guid.Parse("50f14c55-d5f0-4bba-930e-ab73b012e6cb");
    public static readonly Guid ResidentQatar  = Guid.Parse("7b06bc91-88b3-46ec-b6cc-ef2338864d41");
    public static readonly Guid NonQatari  = Guid.Parse("42b374d1-8032-44d7-95bd-ff5a64abbc95");
}
[Table(nameof(CandidateType), Schema = "lkp")]
public class CandidateType : LookupBase
{
    
}
