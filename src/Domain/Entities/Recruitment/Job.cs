using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Domain.Entities.Recruitment;

[Table(nameof(Job), Schema = Schemas.Hr)]
public class Job : EventEntity
{
    public Guid RequestingDepartmentId { get; set; }
    public Department? RequestingDepartment { get; set; }
    [MaxLength(200),Required]
    public required string Title { get; set; }

    public Guid JobCategoryId { get; set; }
    public JobCategory? JobCategory { get; set; }

    public Guid? GenderId { get; set; }
    public Gender? Gender { get; set; }

    public Guid WorkLocationId { get; set; }
    public Sector? WorkLocation { get; set; }

    public Guid MajorId { get; set; }
    public Major? Major { get; set; }

    public Guid WorkTypeId { get; set; }
    public WorkType? WorkType { get; set; }
    [Required]
    public int Vacancies { get; set; }
    [Required]
    public DateTimeOffset Deadline { get; set; }
    [MaxLength(2000),Required]
    public required string Description { get; set; }
    [MaxLength(2000),Required]
    public string? Benefits { get; set; }
    
    public DateTimeOffset? PublishAt { get; set; }

    public Guid StatusId { get; set; }
    public JobStatus? Status { get; set; }
    
    public Guid QuotaId { get; set; }
    public JobQuota? Quota { get; set; }

    public ICollection<Invitation> Invitations { get; init; } = [];
    public ICollection<JobDegree> Degrees { get; init; } = [];
    public ICollection<JobCondition> Conditions { get; init; } = [];
    public ICollection<JobSkill> Skills { get; init; } = [];
}
