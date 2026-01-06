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
public class Job : EventEntity
{

    [Required(ErrorMessage = JobMessages.JobTitleArRequired)]
    [MaxLength(500, ErrorMessage = JobMessages.JobTitleArMaxLength)]
    public required string TitleAr { get; set; }

    [Required(ErrorMessage = JobMessages.JobTitleEnRequired)]
    [MaxLength(500, ErrorMessage = JobMessages.JobTitleEnMaxLength)]
    public required string TitleEn { get; set; }

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
    public DateTimeOffset ClosingDate { get; set; }

    public DateTimeOffset? PublishAt { get; set; }  
    public DateTimeOffset? CancelledAt { get; set; }

    [Required(ErrorMessage = JobMessages.MinimumAgeRequired)]
    public int MinimumAge { get; set; }

    [Required(ErrorMessage = JobMessages.MaximumAgeRequired)]
    public int MaximumAge { get; set; }

    [Required(ErrorMessage = JobMessages.JobStatusRequired)]
    public Guid JobStatusId { get; set; }
    public string? OverViewAr { get; set; }
    public string? OverViewEn { get; set; } 
    public string? BenefitsAr { get; set; } 
    public string? BenefitsEn { get; set; } 
    public string? QualificationDescriptionAr { get; set; }
    public string? QualificationDescriptionEn { get; set; }

    public virtual Sector? Sector { get; set; }
    public virtual Management? Management { get; set; }
    public virtual Department? Department { get; set; }
    public virtual JobCategory? JobCategory { get; set; }
    public virtual TargetEntity? WorkLocation { get; set; }
    public virtual Gender? Gender { get; set; }
    public virtual Major? Major { get; set; }
    public virtual Major? SubMajor { get; set; }
    public virtual WorkType? WorkType { get; set; }
    public virtual JobStatus? JobStatus { get; set; }
    public virtual JobPointsMain? JobPoints { get; set; }
    public virtual JobReviewAttachment? ReviewAttachment { get; set; }
    public virtual List<JobDegree> JobDegrees { get; set; } = [];
    public virtual List<JobCondition> JobConditions { get; set; } = [];
    public virtual List<JobSkill> JobSkills { get; set; } = [];
    public virtual List<JobResponsibility> JobResponsibilities { get; set; } = [];
    public virtual List<JobRequiredAttachment> JobRequiredAttachments { get; set; } = [];
    public virtual List<Invitation> Invitations { get; set; } = [];
    public virtual List<JobTabReviewNote> TabReviewNotes { get; set; } = [];

    public void ChangeStatus(Guid newStatusId)
    {
        this.JobStatusId = newStatusId;
        AddDomainEvent(new JobStatusChangedDomainEvent(Id, newStatusId, DateTimeOffset.UtcNow));
        if(newStatusId == JobStatusIds.Draft)
        {
            AddDomainEvent(new ChangeJobStatusNotificationDomainEvent(this,DateTimeOffset.Now));
        }
    }
}
