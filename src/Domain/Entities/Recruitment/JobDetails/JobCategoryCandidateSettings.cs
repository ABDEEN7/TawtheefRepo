using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

public static class JobCategoryCandidateSettingsIds
{
    public static Guid Default = Guid.Parse("f1f1ce2e-1f0b-42b9-9a52-7996fd4b5c29");
}

[Table(nameof(JobCategoryCandidateSettings), Schema = Schemas.Hr)]
public class JobCategoryCandidateSettings : EventEntity
{
    [Required]
    public int AcademicJobVacancies { get; set; }
    [Required]
    public int LaborJobVacancies { get; set; }
    [Required]
    public int AdministrativeJobVacancies { get; set; }
}
