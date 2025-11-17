using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

public class JobQuota : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public Guid NationalityId { get; set; }
    public Country? Nationality { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Percentage { get; set; }
}
