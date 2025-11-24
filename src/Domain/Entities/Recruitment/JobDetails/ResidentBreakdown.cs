using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;
[Table(nameof(ResidentBreakdown), Schema = Schemas.Hr)]

public class ResidentBreakdown : EventEntity
{
    public Guid JobQuotaId { get; set; }
    public JobQuota? JobQuota { get; set; }
    
    public Guid NationalityId { get; set; }
    public Country? Nationality { get; set; }
    
    public decimal Percentage { get; set; }
}
