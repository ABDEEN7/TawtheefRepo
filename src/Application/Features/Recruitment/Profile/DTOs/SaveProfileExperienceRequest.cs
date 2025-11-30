namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class SaveProfileExperienceRequest
{
    public bool Submit { get; set; }

    public List<ExperienceUpsertDto> Experiences { get; set; } = [];
    public List<TrainingCourseUpsertDto> TrainingCourses { get; set; } = [];
    public List<AchievementUpsertDto> Achievements { get; set; } = [];
}

public sealed class ExperienceUpsertDto
{
    public Guid? Id { get; set; } 
    public string Organization { get; set; } = default!;
    public string Position { get; set; } = default!;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public Guid? CertificateId { get; set; }

    public List<string> Achievements { get; set; } = [];
}

public sealed class TrainingCourseUpsertDto
{
    public Guid? Id { get; set; }
    public required string Organization { get; set; }
    public required string Position { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public Guid? CertificateId { get; set; }
}
public sealed class AchievementUpsertDto
{
    public Guid? Id { get; set; }
    public required string Organization { get; set; }
    public required string Title { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public Guid? CertificateId { get; set; }
}
