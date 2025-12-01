using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

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
    public required string Organization { get; set; }
    public required string Position { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Achievements { get; set; }

    public Guid? CertificateId { get; set; }
    public int? CertificateFileIndex { get; set; }
}

public sealed class TrainingCourseUpsertDto
{
    public Guid? Id { get; set; }
    public required string Organization { get; set; }
    public required string Position { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public Guid? CertificateId { get; set; }
    public int? CertificateFileIndex { get; set; }
}
