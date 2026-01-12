using Microsoft.AspNetCore.Http;

namespace Application.Recruitment.Features.Profile.DTOs.SaveOperation;

public sealed class SaveProfileExperienceRequest
{
    public bool Submit { get; set; }

    public string ExperiencesJson { get; set; } = string.Empty;
    public string TrainingCoursesJson { get; set; } = string.Empty;

    public List<IFormFile> ExperienceFiles { get; set; } = [];
    public List<IFormFile> TrainingCourseFiles { get; set; } = [];
}

public sealed class ExperienceUpsertDto
{
    public Guid? Id { get; set; }
    public required string EmployerName { get; set; }
    public required string JobTitle { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public Guid CountryId { get; set; }
    public string? Description { get; set; }
    public Guid? QualificationId { get; set; }

    public Guid? CertificateId { get; set; }
    public int? CertificateFileIndex { get; set; }
}

public sealed class TrainingCourseUpsertDto
{
    public Guid? Id { get; set; }
    public required string Provider { get; set; }
    public required string Title { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public Guid CountryId { get; set; }
    public string? Description { get; set; }
    public Guid? CertificateId { get; set; }
    public int? CertificateFileIndex { get; set; }
}
