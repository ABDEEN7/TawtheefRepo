using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobPointsDetail), Schema = Schemas.Hr)]
public class JobPointsDetail : EventEntity
{
    [Required]
    public Guid JobPointsMainId { get; set; }

    public virtual JobPointsMain? JobPointsMain { get; set; }

    public JobPointRuleType Type { get; set; }

    // Rule code: e.g. AGE_25_30, DEGREE_BACHELOR, EXPERIENCE_5_PLUS
    [Required]
    [MaxLength(50)]
    public required string Code { get; set; }

    // Display name (optional, localized elsewhere or generic)
    [MaxLength(200)]
    public string? Name { get; set; }

    [Required]
    [Range(0, 100)]
    public int Points { get; set; }

    public Guid? ReferenceId { get; set; }
}

public enum JobPointRuleType
{
    ApplicantCategory,
    Education,
    Training,
    Language,
    Experience,
    Skill,
    Certificate
}
