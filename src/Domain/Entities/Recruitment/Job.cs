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
    // -----------------------------
    // Basic Job Information
    // -----------------------------
    [Required]
    public required string Title { get; set; }

    [Required]
    public DateTimeOffset Deadline { get; set; }

    [MaxLength(2000), Required]
    public required string Description { get; set; }

    [MaxLength(2000), Required]
    public required string Benefits { get; set; }

    [MaxLength(4000)]
    public string? Overview { get; set; }

    [MaxLength(4000)]
    public string? QualificationsDescription { get; set; }

    public DateTimeOffset? PublishAt { get; set; }

    // -----------------------------
    // Requirements
    // -----------------------------
    [Range(0, 150)] // Based on BRD age constraints
    public int MinimumAge { get; set; }

    [Range(0, 150)]
    public int MaximumAge { get; set; }

    [Range(0, int.MaxValue)]
    public int MinimumExperienceYears { get; set; }

    [Range(1, int.MaxValue)] // BRD: "√ﬂ»— „‰ ’›—"
    public int Vacancies { get; set; }

    // -----------------------------
    // Foreign Keys + Navigation
    // -----------------------------
    public Guid SectorId { get; set; }
    public Sector? Sector { get; set; }

    public Guid ManagementId { get; set; }
    public Managment? Management { get; set; }

    public Guid RequestingDepartmentId { get; set; }
    public Department? RequestingDepartment { get; set; }

    public Guid JobCategoryId { get; set; }
    public JobCategory? JobCategory { get; set; }

    public Guid? GenderId { get; set; }
    public Gender? Gender { get; set; }

    public Guid WorkLocationId { get; set; }
    public TargetEntity? WorkLocation { get; set; }

    public Guid MajorId { get; set; }
    public Major? Major { get; set; }

    public Guid? SubMajorId { get; set; }
    public Major? SubMajor { get; set; }

    public Guid WorkTypeId { get; set; }
    public WorkType? WorkType { get; set; }

    public Guid StatusId { get; set; }
    public JobStatus? Status { get; set; }

    public JobQuota? Quota { get; set; }

    // -----------------------------
    // Collections
    // -----------------------------
    public ICollection<Invitation> Invitations { get; set; } = [];
    public ICollection<JobDegree> Degrees { get; set; } = [];
    public ICollection<JobCondition> Conditions { get; set; } = [];
    public ICollection<JobSkill> Skills { get; set; } = [];
    public ICollection<JobResponsibility> Responsibilities { get; set; } = [];
    public ICollection<JobRequiredAttachment> RequiredAttachments { get; set; } = [];
}
