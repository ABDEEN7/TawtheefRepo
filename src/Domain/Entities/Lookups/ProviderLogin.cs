using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class ProviderLoginIds
{
    public static readonly Guid Google = Guid.Parse("0d1ab8a4-2b89-4dcc-aa6f-6ec92ccb887c");
    public static readonly Guid QatarPass  = Guid.Parse("b8854959-1e46-4595-b51f-de3c09e3ed85");
    public static readonly Guid AzureAD  = Guid.Parse("e0575116-ea2b-4917-965f-214048a4c78b");
    public static readonly Guid QatarResidentOtp  = Guid.Parse("fc379bb9-39f7-458a-85f8-6e54d1780178");
}
[Table(nameof(ProviderLogin), Schema = Schemas.Lookup)]
public class ProviderLogin : LookupBase
{
    public virtual ICollection<CandidateTypeProviderLogin> CandidateTypeProviderLogins { get; set; } = [];
}
