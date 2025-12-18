using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class CreateJobDto
{
    [Required(ErrorMessage = JobMessages.JOB_TITLE_AR_REQUIRED)]
    [MaxLength(500)]
    public required string TitleAr { get; set; }

    [Required(ErrorMessage = JobMessages.JOB_TITLE_EN_REQUIRED)]
    [MaxLength(500)]
    public required string TitleEn { get; set; }

    [Required(ErrorMessage = JobMessages.SECTOR_REQUIRED)]
    public Guid SectorId { get; set; }

    [Required(ErrorMessage = JobMessages.MANAGEMENT_REQUIRED)]
    public Guid ManagementId { get; set; }

    [Required(ErrorMessage = JobMessages.DEPARTMENT_REQUIRED)]
    public Guid DepartmentId { get; set; }

    [Required(ErrorMessage = JobMessages.YEARS_EXPERIENCE_REQUIRED)]
    [Range(0, 100)]
    public int YearsOfExperience { get; set; }

    [Required(ErrorMessage = JobMessages.JOB_CATEGORY_REQUIRED)]
    public Guid JobCategoryId { get; set; }

    [Required(ErrorMessage = JobMessages.WORK_LOCATION_REQUIRED)]
    public Guid WorkLocationId { get; set; }

    public Guid? GenderId { get; set; }

    [Required(ErrorMessage = JobMessages.MAJOR_REQUIRED)]
    public Guid MajorId { get; set; }

    public Guid? SubMajorId { get; set; }

    [Required(ErrorMessage = JobMessages.WORK_TYPE_REQUIRED)]
    public Guid WorkTypeId { get; set; }

    [Required(ErrorMessage = JobMessages.VACANCIES_REQUIRED)]
    [Range(1, int.MaxValue)]
    public int NumberOfVacancies { get; set; }

    [Required(ErrorMessage = JobMessages.CLOSING_DATE_REQUIRED)]
    public DateTimeOffset ClosingDate { get; set; }

    [Required(ErrorMessage = JobMessages.MINIMUM_AGE_REQUIRED)]
    public int MinimumAge { get; set; }

    [Required(ErrorMessage = JobMessages.MAXIMUM_AGE_REQUIRED)]
    public int MaximumAge { get; set; }
}
