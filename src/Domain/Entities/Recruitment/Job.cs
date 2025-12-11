using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Domain.Entities.Recruitment;

[Table(nameof(Job), Schema = Schemas.Hr)]
public class Job : EventEntity
{

    [Required(ErrorMessage = JobValidationMessages.JOB_TITLE_AR_REQUIRED)]
    [MaxLength(500, ErrorMessage = JobValidationMessages.JOB_TITLE_AR_MAX_LENGTH)]
    public required string TitleAr { get; set; }

    [Required(ErrorMessage = JobValidationMessages.JOB_TITLE_EN_REQUIRED)]
    [MaxLength(500, ErrorMessage = JobValidationMessages.JOB_TITLE_EN_MAX_LENGTH)]
    public required string TitleEn { get; set; }

    [Required(ErrorMessage = JobValidationMessages.SECTOR_REQUIRED)]
    public Guid SectorId { get; set; }

    [Required(ErrorMessage = JobValidationMessages.MANAGEMENT_REQUIRED)]
    public Guid ManagementId { get; set; }

    [Required(ErrorMessage = JobValidationMessages.DEPARTMENT_REQUIRED)]
    public Guid DepartmentId { get; set; } 

    [Required(ErrorMessage = JobValidationMessages.YEARS_EXPERIENCE_REQUIRED)]
    [Range(0, 100, ErrorMessage = JobValidationMessages.YEARS_EXPERIENCE_RANGE)]
    public int YearsOfExperience { get; set; }

    [Required(ErrorMessage = JobValidationMessages.JOB_CATEGORY_REQUIRED)]
    public Guid JobCategoryId { get; set; }

    [Required(ErrorMessage = JobValidationMessages.WORK_LOCATION_REQUIRED)]
    public Guid WorkLocationId { get; set; }

    public Guid? GenderId { get; set; }

    [Required(ErrorMessage = JobValidationMessages.MAJOR_REQUIRED)]
    public Guid MajorId { get; set; }

    public Guid? SubMajorId { get; set; } 

    [Required(ErrorMessage = JobValidationMessages.WORK_TYPE_REQUIRED)]
    public Guid WorkTypeId { get; set; }

    [Required(ErrorMessage = JobValidationMessages.VACANCIES_REQUIRED)]
    [Range(1, int.MaxValue, ErrorMessage = JobValidationMessages.VACANCIES_GREATER_THAN_ZERO)]
    public int NumberOfVacancies { get; set; }

    [Required(ErrorMessage = JobValidationMessages.CLOSING_DATE_REQUIRED)]
    public DateTimeOffset ClosingDate { get; set; }

    public DateTimeOffset? PublishAt { get; set; }  
    public DateTimeOffset? CancelledAt { get; set; }

    [Required(ErrorMessage = JobValidationMessages.MINIMUM_AGE_REQUIRED)]
    public int MinimumAge { get; set; }

    [Required(ErrorMessage = JobValidationMessages.MAXIMUM_AGE_REQUIRED)]
    public int MaximumAge { get; set; }

    [Required(ErrorMessage = JobValidationMessages.JOB_STATUS_REQUIRED)]
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

    public virtual List<JobDegree> JobDegrees { get; set; } = new List<JobDegree>();
    public virtual List<JobCondition> JobConditions { get; set; } = new List<JobCondition>();
    public virtual List<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    public virtual List<JobResponsibility> JobResponsibilities { get; set; } = new List<JobResponsibility>();
    public virtual List<JobRequiredAttachment> JobRequiredAttachments { get; set; } = new List<JobRequiredAttachment>();
    public virtual List<Invitation> Invitations { get; set; } = new List<Invitation>();

    public virtual JobQuota? JobQuota { get; set; }
}
