using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Domain.Entities.Recruitment;

[Table(nameof(Job), Schema = Schemas.Hr)]
public class Job : EventEntity
{
    public string Title { get; set; } = string.Empty;
    public int Vacancies { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public JobQuotas Quotas { get; set; } = new();
    public Guid RequestingDepartmentId { get; set; }
    public Department? RequestingDepartment { get; set; }
    public Guid MajorId { get; set; }
    public Major? Major { get; set; }
    public Guid WorkTypeId { get; set; }
    public WorkType? WorkType { get; set; }
    public Guid GenderId { get; set; }
    public Gender? Gender { get; set; }
    public Guid TargetEntityId { get; set; }
    public TargetEntity? TargetEntity { get; set; }
    public Guid JobCategoryId { get; set; }
    public JobCategory? JobCategory { get; set; }
    public string? Description { get; set; }
    public string? Benefits { get; set; }
    public Guid StatusId { get; set; }
    public JobStatus? Status { get; set; }
    
    public ICollection<JobSkill> Skills { get; set; } = [];
    public ICollection<JobCondition> Conditions { get; set; } = [];
    public ICollection<JobDegree> Degrees { get; set; } = [];
}
