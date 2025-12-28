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

    [Required]
    public required string Code { get; set; }

    public string? Name { get; set; }

    [Required, Range(0, 100)]
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
