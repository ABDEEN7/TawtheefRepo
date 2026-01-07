using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobCategoryCandidateSettings), Schema = Schemas.Hr)]
public class JobCategoryCandidateSettings : EventEntity
{
    [Required]
    public int AcademicJobVacancies { get; init; }
    [Required]
    public int LaborJobVacancies { get; init; }
    [Required]
    public int AdministrativeJobVacancies { get; init; }
}
