using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobQuota), Schema = Schemas.Hr)]
public class JobQuota : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }
    [Precision(18, 2)]
    public decimal QatariCitizens { get; set; }
    [Precision(18, 2)]
    public decimal QatarMother { get; set; }
    [Precision(18, 2)]
    public decimal NonQatariSpouse { get; set; }
    [Precision(18, 2)]
    public decimal Gcc { get; set; }
    [Precision(18, 2)]
    public decimal QuGrads { get; set; }
    [Precision(18, 2)]
    public decimal Residents { get; set; }
    public ICollection<ResidentBreakdown> ResidentsBreakdowns { get; set; } = [];
}
