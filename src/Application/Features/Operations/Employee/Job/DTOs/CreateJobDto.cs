using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class CreateJobDto
{
    [Required(ErrorMessage = JobMessages.JobTitleArRequired)]
    [MaxLength(500)]
    public required string TitleAr { get; set; }

    [Required(ErrorMessage = JobMessages.JobTitleEnRequired)]
    [MaxLength(500)]
    public required string TitleEn { get; set; }

    [Required(ErrorMessage = JobMessages.SectorRequired)]
    public Guid SectorId { get; set; }

    [Required(ErrorMessage = JobMessages.ManagementRequired)]
    public Guid ManagementId { get; set; }

    [Required(ErrorMessage = JobMessages.DepartmentRequired)]
    public Guid DepartmentId { get; set; }

    [Required(ErrorMessage = JobMessages.YearsExperienceRequired)]
    [Range(0, 100)]
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
    [Range(1, int.MaxValue)]
    public int NumberOfVacancies { get; set; }

    [Required(ErrorMessage = JobMessages.ClosingDateRequired)]
    public DateTimeOffset ClosingDate { get; set; }

    [Required(ErrorMessage = JobMessages.MinimumAgeRequired)]
    public int MinimumAge { get; set; }

    [Required(ErrorMessage = JobMessages.MaximumAgeRequired)]
    public int MaximumAge { get; set; }
}
