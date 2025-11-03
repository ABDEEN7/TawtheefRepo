using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

public class JobQuota : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public Guid NationalityId { get; set; }
    public Country? Nationality { get; set; }
    
    public decimal Percentage { get; set; }
}
