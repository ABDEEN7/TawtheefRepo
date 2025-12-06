using System.ComponentModel.DataAnnotations;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Dtos;

public class CreateJobDto
{
    [Required(ErrorMessage = JobValidationMessages.JOB_TITLE_AR_REQUIRED)]
    [MaxLength(500)]
    public required string TitleAr { get; set; }

    [Required(ErrorMessage = JobValidationMessages.JOB_TITLE_EN_REQUIRED)]
    [MaxLength(500)]
    public required string TitleEn { get; set; }

    [Required(ErrorMessage = JobValidationMessages.SECTOR_REQUIRED)]
    public Guid SectorId { get; set; }

    [Required(ErrorMessage = JobValidationMessages.MANAGEMENT_REQUIRED)]
    public Guid ManagementId { get; set; }

    [Required(ErrorMessage = JobValidationMessages.DEPARTMENT_REQUIRED)]
    public Guid DepartmentId { get; set; }

    [Required(ErrorMessage = JobValidationMessages.YEARS_EXPERIENCE_REQUIRED)]
    [Range(0, 100)]
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
    [Range(1, int.MaxValue)]
    public int NumberOfVacancies { get; set; }

    [Required(ErrorMessage = JobValidationMessages.CLOSING_DATE_REQUIRED)]
    public DateTime ClosingDate { get; set; }

    [Required(ErrorMessage = JobValidationMessages.MINIMUM_AGE_REQUIRED)]
    public int MinimumAge { get; set; }

    [Required(ErrorMessage = JobValidationMessages.MAXIMUM_AGE_REQUIRED)]
    public int MaximumAge { get; set; }

    [MaxLength(5000)]
    public string? OverviewAr { get; set; }
    [MaxLength(5000)]
    public string? OverviewEn { get; set; }

    [MaxLength(5000)]
    public string? BenefitsAr { get; set; }
    [MaxLength(5000)]
    public string? BenefitsEn { get; set; }

    [MaxLength(5000)]
    public string? QualificationsDescriptionAr { get; set; }
    [MaxLength(5000)]
    public string? QualificationsDescriptionEn { get; set; }

    public List<JobDegreeRequestDto>? Degrees { get; set; }
    public List<JobConditionRequestDto>? Conditions { get; set; }
    public List<JobResponsibilityRequestDto>? Responsibilities { get; set; }
    public List<JobSkillRequestDto>? Skills { get; set; }
    public List<JobRequiredAttachmentRequestDto>? RequiredAttachments { get; set; }
    public JobQuotaRequestDto? Quota { get; set; }
}
