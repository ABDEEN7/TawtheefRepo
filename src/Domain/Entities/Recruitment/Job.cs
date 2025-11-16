using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Domain.Entities.Recruitment;

[Table(nameof(Job), Schema = Schemas.Hr)]
public class Job : EventEntity
{
    public Guid RequestingDepartmentId { get; set; }
    public Department? RequestingDepartment { get; set; }

    [Required, MaxLength(250)]
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

    public int Vacancies { get; set; }
    public DateTimeOffset Deadline { get; set; }

    public ICollection<JobDegree> Degrees { get; init; } = new List<JobDegree>();
    public ICollection<JobQuota> Quotas { get; init; } = new List<JobQuota>();
    public ICollection<JobCondition> Conditions { get; init; } = new List<JobCondition>();
    public ICollection<JobSkill> Skills { get; init; } = new List<JobSkill>();
    public string? Description { get; set; }
    public string? Benefits { get; set; }
    
    public DateTimeOffset? PublishAt { get; set; }

    public Guid StatusId { get; set; }
    public JobStatus? Status { get; set; }
    public ICollection<Invitation> Invitations { get; init; } = new List<Invitation>();
}
