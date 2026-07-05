using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class CandidateTypeIds
{
    public static readonly Guid Qatari = Guid.Parse("268d49f6-0dda-4dd6-8346-be355c553496");
    public static readonly Guid GCC  = Guid.Parse("50f14c55-d5f0-4bba-930e-ab73b012e6cb");
    public static readonly Guid SonOfQatariMother  = Guid.Parse("744256ea-4ee0-4a63-b071-8810895a33dc");
    public static readonly Guid WifeOfQatari  = Guid.Parse("ce67465e-6fed-4a83-9161-8eba16a79af3");
    public static readonly Guid ResidentQatar  = Guid.Parse("7b06bc91-88b3-46ec-b6cc-ef2338864d41");
    public static readonly Guid NonQatari  = Guid.Parse("42b374d1-8032-44d7-95bd-ff5a64abbc95");
    public static readonly Guid QidHolder = Guid.Parse("91f86f10-1799-44e8-bc93-83f0c21f50b4");

    public static bool IsVerifiedIdentityLocked(Guid? candidateTypeId) =>
        candidateTypeId == Qatari || candidateTypeId == QidHolder;
}
[Table(nameof(CandidateType), Schema = Schemas.Lookup)]
public class CandidateType : LookupBase
{
    public virtual ICollection<CandidateTypeProviderLogin> CandidateTypeProviderLogins { get; set; } = [];
}
