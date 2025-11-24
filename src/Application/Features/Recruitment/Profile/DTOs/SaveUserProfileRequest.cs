namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class SaveUserProfileRequest
{
    public bool Submit { get; set; } // false = draft, true = final submit

    public Guid CandidateTypeId { get; set; }
    public Guid TargetEntityId { get; set; }
    public int? NationalNumber { get; set; }
    public DateOnly? BirthDate { get; set; }

    public Guid? NationalityId { get; set; }
    public Guid? GenderId { get; set; }
    public Guid? ReligionId { get; set; }
    public Guid? MaritalStatusId { get; set; }
    public int? ChildrenCount { get; set; }

    public Guid? ResidenceCountryId { get; set; }
    public string? Address { get; set; }
    public Guid? InterviewLocationId { get; set; }

    public bool HasDisability { get; set; }
    public string? DisabilityDetails { get; set; }

    public string? SponsorEmployerName { get; set; }
    public string? SponsorEmployerNumber { get; set; }
    public string? SponsorCardName { get; set; }
    public Guid? SponsorTypeId { get; set; }

    public List<QualificationDto> Degrees { get; set; } = [];
    public List<ExperienceDto> Experiences { get; set; } = [];
    public List<TrainingCourseDto> TrainingCourses { get; set; } = [];
    public List<SkillDto> Skills { get; set; } = [];
    public List<LanguageDto> Languages { get; set; } = [];
    public List<AttachmentDto> AdditionalAttachments { get; set; } = [];
}

public class QualificationDto
{
    public Guid QualificationId { get; set; }
    public string? Name { get; set; }
    public string? Grade { get; set; }
}

public class ExperienceDto
{
    public Guid ExperienceId { get; set; }
    public string? CompanyName { get; set; }
    public string? Position { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
public class TrainingCourseDto
{
    public Guid TrainingCourseId { get; set; }
    public string? Organization { get; set; }
}

public class SkillDto
{
    public Guid SkillId { get; set; }
    public string? Name { get; set; }
}
public class LanguageDto
{
    public Guid LanguageId { get; set; }
}
public class AttachmentDto
{
    public Guid AttachmentId { get; set; }
}
