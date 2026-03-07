using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

[Table(nameof(CandidateTypeProviderLogin), Schema = Schemas.Lookup)]
[Index(nameof(CandidateTypeId))]
[Index(nameof(ProviderLoginId))]
public class CandidateTypeProviderLogin
{
    public Guid CandidateTypeId { get; set; }
    public CandidateType? CandidateType { get; set; }
    
    public Guid ProviderLoginId { get; set; }
    public ProviderLogin? ProviderLogin { get; set; }
}
