using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Domain.Events.Operation.Employee.Job;

namespace Tawtheef.Domain.Entities.Recruitment;

[Table(nameof(Job), Schema = Schemas.Hr)]
public sealed class Job : EventEntity
{
    [Required(ErrorMessage = JobMessages.JobTitleArRequired)]
    public Guid JobTitleId { get; set; }

    [Required(ErrorMessage = JobMessages.SectorRequired)]
    public Guid SectorId { get; set; }

    [Required(ErrorMessage = JobMessages.ManagementRequired)]
    public Guid ManagementId { get; set; }

    [Required(ErrorMessage = JobMessages.DepartmentRequired)]
    public Guid DepartmentId { get; set; }

    [Required(ErrorMessage = JobMessages.YearsExperienceRequired)]
    [Range(0, 100, ErrorMessage = JobMessages.YearsExperienceRange)]
    public int YearsOfExperience { get; set; }

    [Required(ErrorMessage = JobMessages.JobCategoryRequired)]
    public Guid JobCategoryId { get; set; }

    [Required(ErrorMessage = JobMessages.WorkLocationRequired)]
    public Guid WorkLocationId { get; set; }

    public Guid? GenderId { get; set; }

    [Required(ErrorMessage = JobMessages.MajorRequired)]
    public Guid MajorId { get; set; }

    public Guid? SubMajorId { get; set; }

    [Required(ErrorMessage = JobMessages.WorkTypeRequired)]
    public Guid WorkTypeId { get; set; }

    [Required(ErrorMessage = JobMessages.VacanciesRequired)]
    [Range(1, int.MaxValue, ErrorMessage = JobMessages.VacanciesGreaterThanZero)]
    public int NumberOfVacancies { get; set; }

    [Required(ErrorMessage = JobMessages.ClosingDateRequired)]
    public DateTime ClosingDate { get; set; }

    public DateTime? PublishAt { get; init; }
    public DateTime? CancelledAt { get; init; }

    [Required(ErrorMessage = JobMessages.MinimumAgeRequired)]
    public int MinimumAge { get; set; }

    [Required(ErrorMessage = JobMessages.MaximumAgeRequired)]
    public int MaximumAge { get; set; }

    [Required(ErrorMessage = JobMessages.JobStatusRequired)]
    public Guid JobStatusId { get; set; }

    // =========================
    // Textual Descriptions
    // =========================

    [MaxLength(4000)]
    public string? OverViewAr { get; set; }

    [MaxLength(4000)]
    public string? OverViewEn { get; set; }

    [MaxLength(2000)]
    public string? BenefitsAr { get; set; }

    [MaxLength(2000)]
    public string? BenefitsEn { get; set; }

    [MaxLength(3000)]
    public string? QualificationDescriptionAr { get; set; }

    [MaxLength(3000)]
    public string? QualificationDescriptionEn { get; set; }

    // =========================
    // Navigation Properties
    // =========================

    public Sector? Sector { get; init; }
    public Management? Management { get; init; }
    public Department? Department { get; init; }
    public JobCategory? JobCategory { get; init; }
    public JobTitle? JobTitle { get; init; }
    public TargetEntity? WorkLocation { get; init; }
    public Gender? Gender { get; init; }
    public Major? Major { get; init; }
    public Major? SubMajor { get; init; }
    public WorkType? WorkType { get; init; }
    public JobStatus? JobStatus { get; init; }
    public JobPointsMain? JobPoints { get; set; }
    public JobCandidateFilterSetting? CandidateFilterSetting { get; init; }
    public JobReviewAttachment? ReviewAttachment { get; init; }

    public List<JobDegree> JobDegrees { get; set; } = [];
    public List<JobCondition> JobConditions { get; set; } = [];
    public List<JobSkill> JobSkills { get; set; } = [];
    public List<JobResponsibility> JobResponsibilities { get; set; } = [];
    public List<JobRequiredAttachment> JobRequiredAttachments { get; set; } = [];
    public List<Invitation> Invitations { get; init; } = [];
    public List<JobTabReviewNote> TabReviewNotes { get; init; } = [];

    public void ChangeStatus(Guid newStatusId)
    {
        JobStatusId = newStatusId;
        if (newStatusId == JobStatusIds.PendingApproval)
            AddDomainEvent(new ChangeJobStatusNotificationDomainEvent(this, DateTimeOffset.Now));
        else if (newStatusId == JobStatusIds.Approved )
            AddDomainEvent(new ChangeJobStatusApprovedNotificationDomainEvent(this, DateTimeOffset.Now));
        else if (newStatusId == JobStatusIds.Rejected)
            AddDomainEvent(new ChangeJobStatusRejectedNotificationDomainEvent(this, DateTimeOffset.Now));
        else if (newStatusId == JobStatusIds.NeedUpdate)
            AddDomainEvent(new ChangeJobStatusNeedUpdateNotificationDomainEvent(this, DateTimeOffset.Now));
    }
}
