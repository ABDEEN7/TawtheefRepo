using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.Recruitment.Features.Profile.DTOs.SaveOperation;

public sealed class SaveProfileExperienceRequest
{
    public bool Submit { get; set; }

    [RegularExpression(@"^[^\<\>]*$", ErrorMessage = "Invalid characters.")]
    public string ExperiencesJson { get; set; } = string.Empty;

    [RegularExpression(@"^[^\<\>]*$", ErrorMessage = "Invalid characters.")]
    public string TrainingCoursesJson { get; set; } = string.Empty;

    public List<IFormFile> ExperienceFiles { get; set; } = [];
    public List<IFormFile> TrainingCourseFiles { get; set; } = [];
}

public sealed class ExperienceUpsertDto
{
    public Guid? Id { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\u0600-\u06FF\s\,\.\-]+$", ErrorMessage = "Employer name contains invalid characters.")]
    public required string EmployerName { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\u0600-\u06FF\s\,\.\-]+$", ErrorMessage = "Job title contains invalid characters.")]
    public required string JobTitle { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public Guid CountryId { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\u0600-\u06FF\s\,\.\-]+$", ErrorMessage = "Description contains invalid characters.")]
    public string? Description { get; set; }
    public Guid? QualificationId { get; set; }

    public Guid? CertificateId { get; set; }
    public int? CertificateFileIndex { get; set; }
}

public sealed class TrainingCourseUpsertDto
{
    public Guid? Id { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\u0600-\u06FF\s\,\.\-]+$", ErrorMessage = "Provider contains invalid characters.")]
    public required string Provider { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\u0600-\u06FF\s\,\.\-]+$", ErrorMessage = "Title contains invalid characters.")]
    public required string Title { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public Guid CountryId { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\u0600-\u06FF\s\,\.\-]+$", ErrorMessage = "Description contains invalid characters.")]
    public string? Description { get; set; }
    public Guid? CertificateId { get; set; }
    public int? CertificateFileIndex { get; set; }
}

