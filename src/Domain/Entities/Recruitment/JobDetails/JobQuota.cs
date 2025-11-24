using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobQuota), Schema = Schemas.Hr)]
public class JobQuota : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }
    
    public decimal QatariCitizens { get; set; }
    
    public decimal QatarMother { get; set; }
    
    public decimal NonQatariSpouse { get; set; }
    
    public decimal Gcc { get; set; }
    
    public decimal QuGrads { get; set; }
    
    public decimal Residents { get; set; }
    
    public ICollection<ResidentBreakdown> ResidentsBreakdowns { get; set; } = [];
}
